using AutoMapper;
using Dispo.Barber.Domain.DTOs.Appointment;
using Dispo.Barber.Domain.DTOs.Chat;
using Dispo.Barber.Domain.DTOs.Customer;
using Dispo.Barber.Domain.DTOs.Schedule;
using Dispo.Barber.Domain.DTOs.Service;
using Dispo.Barber.Domain.DTOs.User;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Domain.Services.Interface;
using Dispo.Barber.Domain.Utils;


namespace Dispo.Barber.Application.AppServices
{
    public class InformationChatService(IUnitOfWork unitOfWork, IMapper mapper, IUserRepository userRepository) : IInformationChatService
    {
        public async Task<InformationChatDTO> GetInformationChatByIdCompanyAsync(CancellationToken cancellationToken, long companyId)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () =>
                {
                    var companyRepository = unitOfWork.GetRepository<ICompanyRepository>();
                    var company = await companyRepository.GetAsync(cancellationToken, companyId);

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
                    var businessUnity = await unitOfWork.GetRepository<IBusinessUnityRepository>().GetAsync(cancellationToken, user.BusinessUnityId.Value);

                    if (businessUnity == null)
                    {
                        throw new Exception($"Barbeiro com o ID {idUser} não possui unidade de negócio.");
                    }
                    var company = await unitOfWork.GetRepository<ICompanyRepository>().GetAsync(cancellationToken, businessUnity.CompanyId);

                    var services = await unitOfWork.GetRepository<IServiceUserRepository>().GetServicesByUserId(idUser);

                    var informationChat = new InformationChatDTO
                    {
                        NameCompany = company.Name,
                        User = user.Status == UserStatus.Active ? new List<UserInformationDTO> { mapper.Map<UserInformationDTO>(user) } : [],
                        Services = mapper.Map<List<ServiceInformationDTO>>(services.Where(s => s.Status == ServiceStatus.Active).ToList()),
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
                throw new Exception($"Ocorreu um erro ao obter os agendamentos do usuário: {ex.Message}", ex);
            }
        }

        public async Task<Dictionary<string, List<string>>> GetAvailableSlotsAsync(CancellationToken cancellationToken, AvailableSlotRequestDto availableSlotRequestDto)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () =>
                {
                    return await GetAvailableSlotsInternalAsync(cancellationToken, availableSlotRequestDto);
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao tentar obter os horários disponíveis.", ex);
            }
        }

        private async Task<Dictionary<string, List<string>>> GetAvailableSlotsInternalAsync(CancellationToken cancellationToken, AvailableSlotRequestDto availableSlotRequestDto)
        {
            var userScheduleRepository = unitOfWork.GetRepository<IScheduleRepository>();
            DayOfWeek dayOfWeek = availableSlotRequestDto.DateTimeSchedule.DayOfWeek;
            var userSchedules = await userScheduleRepository.GetScheduleByUserDayOfWeek(availableSlotRequestDto.IdUser, dayOfWeek);
            var userBreaks = await userRepository.GetEnabledBreaksAsync(cancellationToken, availableSlotRequestDto.IdUser, dayOfWeek);
            var userDayOffs = await userRepository.GetDaysOffAsync(cancellationToken, availableSlotRequestDto.IdUser);
            var dayIsEqual = LocalTime.Now.Date == availableSlotRequestDto.DateTimeSchedule.Date;

            var slots = GetTimeIntervals(availableSlotRequestDto.Duration, userSchedules, userBreaks, userDayOffs, availableSlotRequestDto.DateTimeSchedule, dayIsEqual);
            var availableSlots = InitializeAvailableSlots();

            var appointmentRepository = unitOfWork.GetRepository<IAppointmentRepository>();
            var appointments = await appointmentRepository.GetAppointmentByUserAndDateIdSync(cancellationToken, availableSlotRequestDto.IdUser, availableSlotRequestDto.DateTimeSchedule);

            var occupiedSlots = GetOccupiedSlots(appointments);

            PopulateAvailableSlots(slots, occupiedSlots, availableSlotRequestDto, availableSlots);

            return availableSlots;
        }

        private async Task<Dictionary<string, List<TimeSpan>>> GetAvailableSlotsInternalSuggestedAsync(CancellationToken cancellationToken, AvailableSlotRequestDto availableSlotRequestDto)
        {
            var userScheduleRepository = unitOfWork.GetRepository<IScheduleRepository>();
            DayOfWeek dayOfWeek = availableSlotRequestDto.DateTimeSchedule.DayOfWeek;
            var userSchedules = await userScheduleRepository.GetScheduleByUserDayOfWeek(availableSlotRequestDto.IdUser, dayOfWeek);
            var userBreaks = await userRepository.GetEnabledBreaksAsync(cancellationToken, availableSlotRequestDto.IdUser, dayOfWeek);
            var userDayOffs = await userRepository.GetDaysOffAsync(cancellationToken, availableSlotRequestDto.IdUser);
            var dayIsEqual = LocalTime.Now.Date == availableSlotRequestDto.DateTimeSchedule.Date;

            var slots = GetTimeIntervals(availableSlotRequestDto.Duration, userSchedules, userBreaks, userDayOffs, availableSlotRequestDto.DateTimeSchedule, dayIsEqual);
            var availableSlots = InitializeAvailableSuggestedSlots();

            var appointmentRepository = unitOfWork.GetRepository<IAppointmentRepository>();
            var appointments = await appointmentRepository.GetAppointmentByUserAndDateIdSync(cancellationToken, availableSlotRequestDto.IdUser, availableSlotRequestDto.DateTimeSchedule);

            var occupiedSlots = GetOccupiedSlots(appointments);

            PopulateAvailableSuggestedSlots(slots, occupiedSlots, availableSlotRequestDto, availableSlots);

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

        private Dictionary<string, List<TimeSpan>> InitializeAvailableSuggestedSlots()
        {
            return new Dictionary<string, List<TimeSpan>>
            {
                { "morning", new List<TimeSpan>() },
                { "afternoon", new List<TimeSpan>() },
                { "evening", new List<TimeSpan>() }
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
        }

        private void PopulateAvailableSuggestedSlots(IEnumerable<DateTime> slots, List<(double Start, double End)> occupiedSlots, AvailableSlotRequestDto availableSlotRequestDto, Dictionary<string, List<TimeSpan>> availableSlots)
        {
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
                        var slotTime = slot.TimeOfDay;

                        string period = CategorizePeriod(slot);

                        availableSlots[period].Add(slotTime);
                    }
                }
            }
        }



        public async Task<bool> GetSuggestionAppointmentAsync()
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var AppointmentRepository = unitOfWork.GetRepository<IAppointmentRepository>();
                DateTime referenceDate = DateTime.Now;
                int daysBefore = 60;

                var appointments = await AppointmentRepository.GetFrequentAppointmentsByDaysBeforeAsync(cancellationTokenSource.Token, daysBefore);

                foreach (var appointment in appointments)
                {
                    var customerAppointments = appointment.Customer.Appointments
                    .OrderByDescending(a => a.Date)
                    .Take(5) // Pegar os dois últimos agendamentos
                    .ToList();

                    if (customerAppointments.Count >= 5)
                    {
                        var intervalCounts = new Dictionary<int, int>();

                        for (int i = 0; i < customerAppointments.Count - 1; i++)
                        {
                            var currentAppointment = customerAppointments[i];
                            var nextAppointment = customerAppointments[i + 1];

                            var interval = (int)Math.Round((currentAppointment.Date - nextAppointment.Date).TotalDays);

                            if (intervalCounts.ContainsKey(interval))
                            {
                                intervalCounts[interval]++;
                            }
                            else
                            {
                                intervalCounts[interval] = 1;
                            }
                        }

                        var mostCustumerFrequentInterval = intervalCounts
                        .OrderByDescending(kvp => kvp.Value)
                        .ThenByDescending(kvp => kvp.Key)
                        .First();

                        var averageInterval = Math.Round(intervalCounts
                        .Select(kvp => kvp.Key)
                        .Average());

                        var latestAppointment = customerAppointments[0];
                        var intervalDataReference = Math.Round((referenceDate.Date - latestAppointment.Date).TotalDays);

                        if (intervalDataReference >= averageInterval || intervalDataReference >= mostCustumerFrequentInterval.Key)
                        {

                            var allCustomerAppointments = appointment.Customer.Appointments
                            .OrderBy(a => a.Date)
                            .Select(a => new { a.Date, a.Date.DayOfWeek, a.Customer }) // Selecionar data e dia da semana
                            .ToList();

                            var allCustomerHourAppointments = appointment.Customer.Appointments
                            .OrderBy(a => a.Date) // Ordenar por data
                            .Select(a => new { a.Date, a.Date.Hour, a.Date.Minute }) // Selecionar hora e minuto de cada agendamento
                            .ToList();

                            var groupedDays = allCustomerAppointments
                            .GroupBy(a => a.DayOfWeek)
                            .OrderByDescending(g => g.Count()) // Ordenar pelo número de agendamentos (prioridade na frequência)
                            .ThenByDescending(g => g.Max(a => a.Date))
                            .ToList();

                            var groupedByHourAndMinute = allCustomerHourAppointments
                                .GroupBy(a => new { a.Hour })
                                .Select(g => new
                                {
                                    Time = $"{g.Key.Hour}",
                                    Count = g.Count(),
                                    LatestDate = g.Max(a => a.Date)
                                })
                                .OrderByDescending(g => g.Count)
                                .ThenByDescending(g => g.LatestDate)
                                .ToList();

                            var mostFrequentTime = groupedByHourAndMinute.FirstOrDefault();

                            var mostFrequentDay = groupedDays.FirstOrDefault();

                            var referenceDateOfWeek = referenceDate.AddDays(averageInterval); // duas forma posso colocar a media ou o ultimo agendamento

                            if (referenceDateOfWeek.DayOfWeek == DayOfWeek.Sunday)
                            {
                                referenceDateOfWeek.AddDays(1);
                            }

                            if (referenceDateOfWeek.DayOfWeek != mostFrequentDay.Last().DayOfWeek)
                            {
                                // Se a diferença for de 1 dia, tenta ajustar diretamente
                                int difference = (int)referenceDateOfWeek.DayOfWeek - (int)mostFrequentDay.Last().DayOfWeek;

                                if (Math.Abs(difference) >= 1)
                                {
                                    if ((int)referenceDateOfWeek.DayOfWeek > (int)mostFrequentDay.Last().DayOfWeek)
                                    {

                                        referenceDateOfWeek = referenceDateOfWeek.AddDays(-difference);
                                        if (referenceDateOfWeek <= referenceDate)
                                            referenceDateOfWeek = referenceDateOfWeek.AddDays(Math.Abs(difference * 2));

                                    }
                                    else
                                    {

                                        referenceDateOfWeek = referenceDateOfWeek.AddDays(Math.Abs(difference));
                                    }
                                }
                            }

                            var adjustedReferenceDate = referenceDateOfWeek.Date + mostFrequentTime.LatestDate.TimeOfDay;

                            var topBarberAppointments = appointment.Customer.Appointments
                            .GroupBy(a => a.AcceptedUser) // Agrupar por barbeiro
                            .Select(group => new
                            {
                                Barber = group.Key,
                                TotalAppointments = group.Count() // Contar agendamentos
                            })
                            .OrderByDescending(barber => barber.TotalAppointments) // Ordenar pelos mais agendados
                            .FirstOrDefault();


                            var topServiceAppointments = appointment.Customer.Appointments
                            .SelectMany(a => a.Services)
                            .GroupBy(service => service)
                            .Select(group => new
                            {
                                Service = group.Key,
                                TotalAppointments = group.Count()
                            })
                            .OrderByDescending(service => service.TotalAppointments)
                            .FirstOrDefault();

                            var availableSlotRequestDto = new AvailableSlotRequestDto
                            {
                                Duration = topServiceAppointments.Service.Service.Duration,
                                IdUser = topBarberAppointments.Barber.Id,
                                DateTimeSchedule = adjustedReferenceDate.Date
                            };


                            // Obtém os horários disponíveis para o barbeiro
                            var availableSlots = await GetAvailableSlotsInternalSuggestedAsync(cancellationTokenSource.Token, availableSlotRequestDto);

                            // Converte o horário formatado (10:30) em TimeSpan para cálculos
                            var formtHour = adjustedReferenceDate.TimeOfDay;

                            // Calcula o horário de término com base na duração do serviço
                            var serviceDuration = TimeSpan.FromMinutes(topServiceAppointments.Service.Service.Duration);
                            var endHour = formtHour + serviceDuration;

                            // Verifica se há algum horário disponível em que o serviço pode ser encaixado
                            var hasTime = availableSlots.Any(slot =>
                            {
                                var available = false;

                                foreach (var slotTime in slot.Value)
                                {

                                    available = slotTime >= formtHour;

                                    if (available)
                                    {
                                        available = endHour <= slotTime.Add(serviceDuration);

                                        if (available)
                                            break;
                                    }
                                }

                                return available;
                            });

                            if (hasTime)
                            {
                                var createAppointmentDTO = new CreateAppointmentDTO
                                {
                                    Date = adjustedReferenceDate,
                                    CustomerObservation = $"Horário sugerido automaticamente pelo sistema. O último agendamento registrado foi em: {DateTime.SpecifyKind(latestAppointment.Date, DateTimeKind.Utc):yyyy-MM-dd HH:mm:ss}.",
                                    AcceptedUserObservation = string.Empty,
                                    AcceptedUserId = topBarberAppointments.Barber.Id,
                                    BusinessUnityId = topBarberAppointments.Barber.BusinessUnityId.Value,
                                    Services = new List<long> { topServiceAppointments.Service.Service.Id },
                                    Status = AppointmentStatus.Suggested,
                                    Customer = mapper.Map<CustomerDTO>(appointment.Customer),
                                };

                                //await appointmentAppService.CreateAsync(cancellationTokenSource.Token, createAppointmentDTO);

                                return true;   // da maneira que esta no momento ao rodar o job ele vai fazer somente uma sugestão
                                               // se caso tiver mais tem que esperar a proxima vez que rodar

                            }
                        }

                    }
                }

                return false;
            }
            catch (Exception)
            {
                return false;
                throw;
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

            bool isDayOff = dayOffs.Any(d =>
                d.DayOff &&
                d.StartDay.HasValue &&
                d.EndDay.HasValue &&
                today >= d.StartDay.Value.Date &&
                today <= d.EndDay.Value.Date);

            if (isDayOff)
            {
                return timeIntervals;
            }

            foreach (var userSchedule in userSchedules)
            {
                if (userSchedule.DayOfWeek != currentDay || userSchedule.DayOff)
                    continue;

                TimeSpan startTime = TimeSpan.Parse(userSchedule.StartDate);
                TimeSpan endTime = TimeSpan.Parse(userSchedule.EndDate);

                TimeSpan currentTime = startTime;
                TimeSpan nowTime = LocalTime.Now.TimeOfDay;

                while (currentTime < endTime)
                {
                    DateTime interval = DateTime.Today.Add(currentTime);

                    if (dayIsEqual && currentTime < nowTime)
                    {
                        currentTime = currentTime.Add(TimeSpan.FromMinutes(duration));
                        continue;
                    }

                    bool isInBreak = breaks.Any(b =>
                    {
                        if (b.DayOfWeek != currentDay) return false;

                        TimeSpan breakStart = TimeSpan.Parse(b.StartDate);
                        TimeSpan breakEnd = TimeSpan.Parse(b.EndDate);
                        return currentTime >= breakStart && currentTime < breakEnd;
                    });

                    if (!isInBreak)
                    {
                        timeIntervals.Add(interval);
                    }

                    currentTime = currentTime.Add(TimeSpan.FromMinutes(duration));
                }
            }

            return timeIntervals;
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

        public async Task<InformationAppointmentChatDTO> GetInformationAppointmentChatByIdAppointment(CancellationToken cancellationToken, long idAppointment)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () =>
                {
                    var appointmentRepository = unitOfWork.GetRepository<IAppointmentRepository>();
                    var appointment = await appointmentRepository.GetAsync(cancellationToken, idAppointment);

                    var businessUnity = await unitOfWork.GetRepository<IBusinessUnityRepository>().GetAsync(cancellationToken, appointment.BusinessUnityId);

                    if (businessUnity == null)
                    {
                        throw new Exception($"Unidade de negocio não encontrada para esse agendamento {idAppointment}.");
                    }

                    var customer = await unitOfWork.GetRepository<ICustomerRepository>().GetAsync(cancellationToken, appointment.CustomerId);

                    if (customer == null)
                    {
                        throw new Exception($"Cliente não encontrada para esse agendamento {idAppointment}.");
                    }

                    var company = await unitOfWork.GetRepository<ICompanyRepository>().GetAsync(cancellationToken, businessUnity.CompanyId);

                    if (company == null)
                    {
                        throw new Exception($"Empresa não encontrada para esse agendamento {idAppointment}.");
                    }

                    var user = await unitOfWork.GetRepository<IUserRepository>().GetAsync(cancellationToken, (long)appointment.AcceptedUserId);

                    if (user == null)
                    {
                        throw new Exception($"Empresa não encontrada para esse agendamento {idAppointment}.");
                    }

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
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter informações do chat para o agendamento com ID {idAppointment}.", ex);
            }
        }
    }
}
