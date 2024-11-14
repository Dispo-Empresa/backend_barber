using AutoMapper;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO.Chat;
using Dispo.Barber.Domain.DTO.Schedule;
using Dispo.Barber.Domain.DTO.Service;
using Dispo.Barber.Domain.DTO.User;
using Dispo.Barber.Domain.Entities;


namespace Dispo.Barber.Application.Service
{
    public class InformationChatService(IUnitOfWork unitOfWork, IMapper mapper) : IinformationChatService
    {
        public async Task<InformationChatDTO> GetInformationChatByIdCompanyAsync(CancellationToken cancellationToken,long companyId)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () =>
                {
                    var companyRepository = unitOfWork.GetRepository<ICompanyRepository>();
                    var company = await companyRepository.GetAsync(cancellationToken,companyId);

                    if (company == null)
                    {
                        throw new Exception($"Empresa com o ID {companyId} não encontrada.");
                    }

                    var businessIdTask = await unitOfWork.GetRepository<IBusinessUnityRepository>().GetIdByCompanyAsync(company.Id);
                    var usersTask = await unitOfWork.GetRepository<IUserRepository>().GetUserByBusinessAsync(businessIdTask);
                    var companyServicesTask = await companyRepository.GetServicesByCompanyAsync(company.Id);

                    var listServices = await unitOfWork.GetRepository<IServiceRepository>().GetListServiceAsync(companyServicesTask);

                    var informationChat = new InformationChatDTO
                    {
                        NameCompany = company.Name,
                        User = mapper.Map<List<UserInformationDTO>>(usersTask),
                        Services = mapper.Map<List<ServiceInformationDTO>>(listServices),
                        BusinessUnities = businessIdTask

                    };

                    return informationChat;
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter informações do chat para a empresa com ID {companyId}.", ex);
            }
        }

        public async Task<InformationChatDTO> GetInformationChatByIdUser(CancellationToken cancellationToken, long idUser)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () =>
                {
                    var userRepository = unitOfWork.GetRepository<IUserRepository>();
                    var user = await userRepository.GetAsync(cancellationToken, idUser);

                if (!user.BusinessUnityId.HasValue)
                {
                    throw new Exception($"Barbeiro com o ID {idUser} não possui unidade de negócio.");
                }
                    var businessUnity =  await unitOfWork.GetRepository<IBusinessUnityRepository>().GetAsync(cancellationToken, user.BusinessUnityId.Value);

                if (businessUnity == null)
                {
                    throw new Exception($"Barbeiro com o ID {idUser} não possui unidade de negócio.");
                }
                    var company = await unitOfWork.GetRepository<ICompanyRepository>().GetAsync(cancellationToken, businessUnity.CompanyId);

                    var services = await unitOfWork.GetRepository<IServiceUserRepository>().GetServicesByUserId(idUser);

                    var informationChat = new InformationChatDTO
                    {
                        NameCompany = company.Name,
                        User = new List<UserInformationDTO> { mapper.Map<UserInformationDTO>(user) },
                        Services = mapper.Map<List<ServiceInformationDTO>>(services),
                        BusinessUnities = user.BusinessUnityId.Value
                    };

                    return informationChat;

                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter informações do chat para barbeiro com ID {idUser}.", ex);
            }
        }

        public async Task<InformationChatUserDTO> GetInformationChatByIdService(List<long> idServices)
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();

                var userList = await unitOfWork.QueryUnderTransactionAsync(cancellationTokenSource.Token, async () =>
                {
                    var serviceRepository = unitOfWork.GetRepository<IServiceUserRepository>();
                    return await serviceRepository.GetUsersByServiceId(idServices);
                });

                var informationChat = new InformationChatUserDTO
                {
                    User = mapper.Map<List<UserInformationDTO>>(userList)
                };

                return informationChat;

            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter informações do chat para barbeiro com ID {1}.", ex);
            }
        }

        public async Task<List<DayScheduleDto>> GetUserAppointmentsByUserIdAsync(CancellationToken cancellationToken, long idUser)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () =>
                {
                    var userScheduleRepository = unitOfWork.GetRepository<IScheduleRepository>();
                    var userSchedules = await userScheduleRepository.GetScheduleByUserId(idUser);

                    var scheduleList = userSchedules.Select(schedule => new DayScheduleDto
                    {
                        DayOfWeek = GetDayOfWeekString((int)schedule.DayOfWeek)
                    }).ToList();

                    return scheduleList;
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao obter os agendamentos do usuário.", ex);
            }
        }

        public async Task<Dictionary<string, List<string>>> GetAvailableSlotsAsync(CancellationToken cancellationToken, AvailableSlotRequestDto availableSlotRequestDto)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () =>
                {
                    var userScheduleRepository = unitOfWork.GetRepository<IScheduleRepository>();
                    DayOfWeek dayOfWeek = availableSlotRequestDto.DateTimeSchedule.DayOfWeek;
                    var userSchedules = await userScheduleRepository.GetScheduleByUserDayOfWeek(availableSlotRequestDto.IdUser, dayOfWeek);
                    var dayIsEqual = DateTime.Today.Date == availableSlotRequestDto.DateTimeSchedule.Date;
                   

                    var slots = GetTimeIntervals(availableSlotRequestDto.Duration, userSchedules, dayIsEqual);
                    var availableSlots = new Dictionary<string, List<string>>
                    {
                        { "morning", new List<string>() },
                        { "afternoon", new List<string>() },
                        { "evening", new List<string>() }
                    };

                    var AppointmentRepository = unitOfWork.GetRepository<IAppointmentRepository>();
                    var AppointmentServiceRepository = unitOfWork.GetRepository<IServiceAppointmentRepository>();

                    var appointments = await AppointmentRepository.GetAppointmentByUserAndDateIdSync(cancellationToken, availableSlotRequestDto.IdUser, availableSlotRequestDto.DateTimeSchedule);

                    List<(double Start, double End)> occupiedSlots = new List<(double, double)>();

                    foreach (var appointment in appointments)
                    {
                        var dateStart = appointment.Date.TimeOfDay;
                        var timeMinuteStart = dateStart.TotalMinutes;
                        var duration = SumDurationService(appointment.Services);
                        var dateEnd = timeMinuteStart + duration;

                        occupiedSlots.Add((timeMinuteStart, dateEnd));
                    }

                    foreach (var slot in slots)
                    {
                        var slotTimeInMinutes = slot.TimeOfDay.TotalMinutes;

                        bool isSlotOccupied = occupiedSlots.Any(occupied =>
                            slotTimeInMinutes >= occupied.Start && slotTimeInMinutes < occupied.End);

                        if (!isSlotOccupied)
                        {
                            var slotEndTime = slotTimeInMinutes + availableSlotRequestDto.Duration;

                            bool isSlotAvailable = !occupiedSlots.Any(occupied =>
                                slotEndTime > occupied.Start && slotEndTime <= occupied.End);

                            if (isSlotAvailable)
                            {
                                string slotTime = slot.TimeOfDay.ToString(@"hh\:mm");

                                string period = CategorizePeriod(slot);

                                availableSlots[period].Add(slotTime);
                            }
                        }
                    }

                    return availableSlots;
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao tentar obter os horários disponíveis.", ex);
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


        private List<DateTime> GetTimeIntervals(int duration, List<UserSchedule> userSchedules, bool dayIsEqual)
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

                DateTime currentDateTime = DateTime.Now;
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
            {
                duration += serviceAppointment.Service.Duration;
            }
            return duration;
        }
        

        private string CategorizePeriod(DateTime slot)
        {
            var hour = slot.Hour;

            if (hour >= 6 && hour < 12)
            {
                return "morning";
            }
            else if (hour >= 12 && hour < 18)
            {
                return "afternoon";
            }
            else
            {
                return "evening";
            }
        }


    }
}
