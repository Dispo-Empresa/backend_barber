using AutoMapper;
using Dispo.Barber.Domain.DTOs.Chat;
using Dispo.Barber.Domain.DTOs.Schedule;
using Dispo.Barber.Domain.DTOs.Service;
using Dispo.Barber.Domain.DTOs.User;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Exceptions;
using Dispo.Barber.Domain.Integration.HubClient;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Domain.Services.Interface;
using Dispo.Barber.Domain.Utils;


namespace Dispo.Barber.Domain.Services
{
    public class InformationChatService(IUnitOfWork unitOfWork, IMapper mapper, IUserRepository userRepository, IHubIntegration hubIntegration) : IInformationChatService
    {
        public async Task<InformationChatDTO> GetInformationChatByIdCompanyAsync(CancellationToken cancellationToken, long companyId)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () =>
                {
                    var companyRepository = unitOfWork.GetRepository<ICompanyRepository>();
                    var businessUnityRepository = unitOfWork.GetRepository<IBusinessUnityRepository>();
                    var serviceRepository = unitOfWork.GetRepository<IServiceRepository>();

                    var company = await companyRepository.GetAsync(cancellationToken, companyId)
                        ?? throw new NotFoundException("Empresa com o ID não encontrada.");

                    var businessUnityId = await businessUnityRepository.GetIdByCompanyAsync(company.Id);
                    if (businessUnityId == 0)
                        throw new BusinessException($"Empresa '{company.Name}' não possui unidade de negócio vinculada.");

                    var users = await userRepository.GetUserByBusinessAsync(businessUnityId);
                    var companyServices = await companyRepository.GetServicesByCompanyAsync(company.Id);
                    var services = await serviceRepository.GetListServiceAsync(companyServices);

                    return new InformationChatDTO
                    {
                        NameCompany = company.Name,
                        User = mapper.Map<List<UserInformationDTO>>(users),
                        Services = mapper.Map<List<ServiceInformationDTO>>(services),
                        BusinessUnities = businessUnityId
                    };
                });
            }
            catch (Exception ex) when (ex is not BusinessException and not NotFoundException)
            {
                throw;
            }
        }

        public async Task<InformationChatDTO> GetInformationChatByIdUser(CancellationToken cancellationToken, long idUser)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () =>
                {
                    var businessUnityRepository = unitOfWork.GetRepository<IBusinessUnityRepository>();
                    var companyRepository = unitOfWork.GetRepository<ICompanyRepository>();
                    var serviceUserRepository = unitOfWork.GetRepository<IServiceUserRepository>();

                    var user = await userRepository.GetAsync(cancellationToken, idUser)
                        ?? throw new NotFoundException("Usuário não encontrado.");

                    if (!user.BusinessUnityId.HasValue)
                        throw new Exception("Barbeiro não possui unidade de negócio.");

                    var businessUnity = await businessUnityRepository.GetAsync(cancellationToken, user.BusinessUnityId.Value)
                        ?? throw new NotFoundException("Unidade de negócio não encontrada.");

                    var company = await companyRepository.GetAsync(cancellationToken, businessUnity.CompanyId)
                        ?? throw new NotFoundException("Empresa não encontrada.");

                    var planType = await hubIntegration.GetPlanType(cancellationToken, company.Id);
                    if (planType == PlanType.BarberFree)
                        throw new BusinessException("O plano da empresa não contempla esta funcionalidade.");

                    var services = await serviceUserRepository.GetServicesByUserId(idUser);
                    var activeServices = services
                        .Where(s => s.Status == ServiceStatus.Active)
                        .ToList();

                    return new InformationChatDTO
                    {
                        NameCompany = company.Name,
                        User = user.Status == UserStatus.Active
                            ? new List<UserInformationDTO> { mapper.Map<UserInformationDTO>(user) }
                            : [],
                        Services = mapper.Map<List<ServiceInformationDTO>>(activeServices),
                        BusinessUnities = user.BusinessUnityId.Value
                    };
                });
            }
            catch (Exception ex) when (ex is not BusinessException and not NotFoundException)
            {
                throw;
            }
        }

        public async Task<InformationChatUserDTO> GetInformationChatByIdService(CancellationToken cancellationToken, List<long> idServices)
        {
            if (idServices == null || idServices.Count == 0)
                throw new BusinessException("É necessário informar pelo menos um serviço.");

            var userList = await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () =>
            {
                var serviceRepository = unitOfWork.GetRepository<IServiceUserRepository>();
                return await serviceRepository.GetUsersByServiceId(idServices);
            });

            return new InformationChatUserDTO
            {
                User = mapper.Map<List<UserInformationDTO>>(userList)
            };
        }

        public async Task<List<DayScheduleDto>> GetUserAppointmentsByUserIdAsync(CancellationToken cancellationToken, long idUser)
        {
            var userScheduleRepository = unitOfWork.GetRepository<IScheduleRepository>();

            var userSchedules = await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () =>
            {
                return await userScheduleRepository.GetScheduleByUserId(idUser);
            });

            return userSchedules
                    .Select(schedule => new DayScheduleDto
                    {
                        DayOfWeek = GetDayOfWeekString((int)schedule.DayOfWeek)
                    })
                    .ToList();
        }

        public async Task<Dictionary<string, List<string>>> GetAvailableSlotsAsync(CancellationToken cancellationToken, AvailableSlotRequestDto availableSlotRequestDto)
        {
            var userScheduleRepository = unitOfWork.GetRepository<IScheduleRepository>();
            var appointmentRepository = unitOfWork.GetRepository<IAppointmentRepository>();

            var dayOfWeek = availableSlotRequestDto.DateTimeSchedule.DayOfWeek;
            var userSchedules = await userScheduleRepository.GetScheduleByUserDayOfWeek(availableSlotRequestDto.IdUser, dayOfWeek);
            var userBreaks = await userRepository.GetEnabledBreaksAsync(cancellationToken, availableSlotRequestDto.IdUser, dayOfWeek);
            var userDayOffs = await userRepository.GetDaysOffAsync(cancellationToken, availableSlotRequestDto.IdUser);

            var isToday = LocalTime.Now.Date == availableSlotRequestDto.DateTimeSchedule.Date;

            var slots = GetTimeIntervals(
                availableSlotRequestDto.Duration,
                userSchedules,
                userBreaks,
                userDayOffs,
                availableSlotRequestDto.DateTimeSchedule,
                isToday);

            var availableSlots = InitializeAvailableSlots();

            var appointments = await appointmentRepository.GetAppointmentByUserAndDateIdSync(
                cancellationToken,
                availableSlotRequestDto.IdUser,
                availableSlotRequestDto.DateTimeSchedule);

            var occupiedSlots = GetOccupiedSlots(appointments);

            PopulateAvailableSlots(slots, occupiedSlots, availableSlotRequestDto, availableSlots);

            return availableSlots;
        }

        private Dictionary<string, List<string>> InitializeAvailableSlots()
        {
            return new Dictionary<string, List<string>>
            {
                { "morning", new List<string>() },
                { "afternoon", new List<string>() },
                { "evening", new List<string>() }
            };
        }

        private List<(double Start, double End)> GetOccupiedSlots(IEnumerable<Appointment> appointments)
        {
            var occupiedSlots = new List<(double Start, double End)>();

            foreach (var appointment in appointments)
            {
                var dateStart = appointment.Date.TimeOfDay;
                var timeMinuteStart = dateStart.TotalMinutes;
                var duration = SumDurationService(appointment.Services);
                var dateEnd = timeMinuteStart + duration;

                occupiedSlots.Add((timeMinuteStart, dateEnd));
            }

            return occupiedSlots;
        }

        private void PopulateAvailableSlots(IEnumerable<DateTime> slots, List<(double Start, double End)> occupiedSlots, AvailableSlotRequestDto availableSlotRequestDto, Dictionary<string, List<string>> availableSlots)
        {
            foreach (var slot in slots)
            {
                var slotTimeInMinutes = slot.TimeOfDay.TotalMinutes;

                bool isSlotOccupied = occupiedSlots.Any(occupied => slotTimeInMinutes >= occupied.Start && slotTimeInMinutes < occupied.End);

                if (!isSlotOccupied)
                {
                    var slotEndTime = slotTimeInMinutes + availableSlotRequestDto.Duration;

                    bool isSlotAvailable = !occupiedSlots.Any(occupied => slotEndTime > occupied.Start && slotEndTime <= occupied.End);

                    if (isSlotAvailable)
                    {
                        string slotTime = slot.TimeOfDay.ToString(@"hh\:mm");

                        string period = CategorizePeriod(slot);

                        availableSlots[period].Add(slotTime);
                    }
                }
            }
        }

        private string GetDayOfWeekString(int dayOfWeek)
        {
            return dayOfWeek switch
            {
                0 => "Dom.",
                1 => "Seg.",
                2 => "Ter.",
                3 => "Qua.",
                4 => "Qui.",
                5 => "Sex.",
                6 => "Sab.",
                _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek), "Dia da semana inválido")
            };
        }

        private List<DateTime> GetTimeIntervals(int duration,List<UserSchedule> userSchedules, List<UserSchedule> breaks, List<UserSchedule> dayOffs, DateTime selectedDate, bool dayIsEqual)
        {
            var timeIntervals = new List<DateTime>();
            var today = selectedDate.Date;
            var currentDay = selectedDate.DayOfWeek;
            var nowTime = LocalTime.Now.TimeOfDay;

            if (IsDayOff(today, dayOffs))
                return timeIntervals;

            foreach (var schedule in userSchedules)
            {
                if (schedule.DayOfWeek != currentDay || schedule.DayOff)
                    continue;

                var startTime = TimeSpan.Parse(schedule.StartDate);
                var endTime = TimeSpan.Parse(schedule.EndDate);

                for (var currentTime = startTime; currentTime < endTime; currentTime = currentTime.Add(TimeSpan.FromMinutes(duration)))
                {
                    if (dayIsEqual && currentTime < nowTime)
                        continue;

                    if (IsInBreak(currentDay, currentTime, breaks))
                        continue;

                    var interval = DateTime.Today.Add(currentTime); //Se algum dia der problema por causa disso, user o de baixo
                    //var interval = selectedDate.Date.Add(currentTime);
                    timeIntervals.Add(interval);
                }
            }

            return timeIntervals;
        }

        private bool IsInBreak(DayOfWeek dayOfWeek, TimeSpan time, List<UserSchedule> breaks)
        {
            return breaks.Any(b =>
                b.DayOfWeek == dayOfWeek &&
                TimeSpan.Parse(b.StartDate) <= time &&
                time < TimeSpan.Parse(b.EndDate));
        }

        private bool IsDayOff(DateTime date, List<UserSchedule> dayOffs)
        {
            return dayOffs.Any(d =>
                d.DayOff &&
                d.StartDay.HasValue &&
                d.EndDay.HasValue &&
                date >= d.StartDay.Value.Date &&
                date <= d.EndDay.Value.Date);
        }

        private List<DateTime> GetTimeIntervalsTwo(int duration, IList<UserSchedule> userSchedules, bool dayIsEqual)
        {
            var timeIntervals = new List<DateTime>();
            foreach (var userSchedule in userSchedules)
            {

                if (userSchedule.DayOff)
                {
                    return timeIntervals;
                }

                TimeSpan startTime = TimeSpan.Parse(userSchedule.StartDate);
                TimeSpan endTime = TimeSpan.Parse(userSchedule.EndDate);

                TimeSpan currentTime = startTime;

                DateTime currentDateTime = LocalTime.Now;
                TimeSpan currentTimeSpan = currentDateTime.TimeOfDay;

                while (currentTime < endTime)
                {
                    if (!userSchedule.IsRest)
                    {
                        DateTime intervalDateTime = DateTime.Today.Add(currentTime);

                        if (dayIsEqual && !(currentTime >= currentTimeSpan))
                        {
                            currentTime = currentTime.Add(TimeSpan.FromMinutes(duration));
                            continue;
                        }
                        else
                        {
                            timeIntervals.Add(intervalDateTime);
                        }


                    }
                    else
                    {
                        DateTime restStartTime = DateTime.Today.Add(startTime);
                        DateTime restEndTime = DateTime.Today.Add(endTime);

                        timeIntervals.RemoveAll(slot => slot >= restStartTime && slot < restEndTime);
                        break;

                    }

                    currentTime = currentTime.Add(TimeSpan.FromMinutes(duration));
                }
            }

            return timeIntervals;
        }

        private int SumDurationService(List<ServiceAppointment> serviceAppointments)
        {
            int duration = 0;
            foreach (var serviceAppointment in serviceAppointments)
                duration += serviceAppointment.Service.Duration;
            return duration;
        }

        private string CategorizePeriod(DateTime slot)
        {
            var hour = slot.Hour;

            if (hour >= 6 && hour < 12)
                return "morning";
            else if (hour >= 12 && hour < 18)
                return "afternoon";
            else
                return "evening";
        }

        public async Task<InformationAppointmentChatDTO> GetInformationAppointmentChatByIdAppointment(CancellationToken cancellationToken, long idAppointment)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () =>
                {
                    var appointmentRepository = unitOfWork.GetRepository<IAppointmentRepository>();

                    var appointment = await appointmentRepository.GetAsync(cancellationToken, idAppointment) ?? throw new NotFoundException("Agendamento não encontrado.");
                    var businessUnity = await unitOfWork.GetRepository<IBusinessUnityRepository>().GetAsync(cancellationToken, appointment.BusinessUnityId) ?? throw new NotFoundException("Unidade de negocio não encontrada para esse agendamento.");
                    var customer = await unitOfWork.GetRepository<ICustomerRepository>().GetAsync(cancellationToken, appointment.CustomerId) ?? throw new NotFoundException("Cliente não encontrado para esse agendamento.");
                    var company = await unitOfWork.GetRepository<ICompanyRepository>().GetAsync(cancellationToken, businessUnity.CompanyId) ?? throw new NotFoundException("Empresa não encontrada para esse agendamento.");
                    var user = await unitOfWork.GetRepository<IUserRepository>().GetAsync(cancellationToken, (long)appointment.AcceptedUserId) ?? throw new NotFoundException("Usuário não encontrado para esse agendamento.");

                    var informationChat = new InformationAppointmentChatDTO
                    {
                        NameCompany = company.Name,
                        IdUser = user.Status == UserStatus.Active ? user.Id : null,
                        NameUser = user.Name,
                        NameCustomer = customer.Name,
                        Phone = customer.Phone,
                        DateAppointment = appointment.Date
                    };

                    return informationChat;

                });
            }
            catch (Exception ex) when (ex is not NotFoundException)
            {
                throw;
            }
        }
    }
}
