using AutoMapper;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO.Appointment;
using Dispo.Barber.Domain.DTO.Chat;
using Dispo.Barber.Domain.DTO.Schedule;
using Dispo.Barber.Domain.DTO.Service;
using Dispo.Barber.Domain.DTO.User;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enum;
using Dispo.Barber.Domain.Extension;
using Dispo.Barber.Domain.Utils;
using System;
using System.Collections.Generic;


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

        public async Task<Dictionary<string, List<string>>> GetSuggestionAppointment()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var AppointmentRepository = unitOfWork.GetRepository<IAppointmentRepository>();
            DateTime referenceDate = DateTime.Now;
            int daysBefore = 60;
            var datesAppointments = new Dictionary<string, List<string>>
            {
                { "dados que serão feita sugestão", new List<string>() },
                { "sugestão", new List<string>() }
            };

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
                        .Select(a => new { a.Date, DayOfWeek = a.Date.DayOfWeek, a.Customer }) // Selecionar data e dia da semana
                        .ToList();

                        var allCustomerHourAppointments = appointment.Customer.Appointments
                        .OrderBy(a => a.Date) // Ordenar por data
                        .Select(a => new { a.Date, Hour = a.Date.Hour, Minute = a.Date.Minute }) // Selecionar hora e minuto de cada agendamento
                        .ToList();

                        var groupedDays = allCustomerAppointments
                        .GroupBy(a => a.DayOfWeek)
                        .OrderByDescending(g => g.Count()) // Ordenar pelo número de agendamentos (prioridade na frequência)
                        .ThenByDescending(g => g.Max(a => a.Date)) 
                        .ToList();

                        var groupedByHourAndMinute = allCustomerHourAppointments
                            .GroupBy(a => new { a.Hour, a.Minute }) 
                            .Select(g => new {
                                Time = $"{g.Key.Hour}:{g.Key.Minute:D2}", 
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
                                        referenceDateOfWeek = referenceDateOfWeek.AddDays(Math.Abs(difference*2));

                                }
                                else
                                {
                                   
                                    referenceDateOfWeek = referenceDateOfWeek.AddDays(Math.Abs(difference));
                                }
                            }
                        }



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


                        if (!topServiceAppointments.Service.Service.UserServices.Any(x => x.UserId == topBarberAppointments.Barber.Id))
                        {
                            topBarberAppointments = null;
                        }

                        
                        if (topBarberAppointments == null)
                        {
                            var newUserByService = topServiceAppointments.Service.Service.UserServices.FirstOrDefault();

                            
                        }



                        var adjustedReferenceDate = new List<string>
                        {
                            $"Ultimo agendamento: {DateTime.SpecifyKind(latestAppointment.Date, DateTimeKind.Utc):yyyy-MM-dd HH:mm:ss}" + Environment.NewLine +
                            $"Sugestão feita: {DateTime.SpecifyKind(referenceDateOfWeek.Date + mostFrequentTime.LatestDate.TimeOfDay, DateTimeKind.Utc):yyyy-MM-dd HH:mm:ss}" + Environment.NewLine +
                            $"Barbeiro: {topBarberAppointments.Barber.Name}" + Environment.NewLine +
                            $"Serviço: {topServiceAppointments.Service.Service.Description}" + Environment.NewLine +
                            $"Cliente: {appointment.Customer.Name}"
                        };


                        // var adjustedReferenceDate = (referenceDateOfWeek.Date + mostFrequentTime.LatestDate.TimeOfDay);
                        var teste = customerAppointments
                        .Select(x => x.Date.ToString("yyyy-MM-dd HH:mm:ss")) 
                        .OrderBy(x => x) 
                        .ToList();

                        datesAppointments["dados que serão feita sugestão"].AddRange(teste);
                        datesAppointments["sugestão"].AddRange(adjustedReferenceDate);

                        return datesAppointments;
                        // ja tenho a data e o horario
                        // verificar agora se essa data que ele esta preparando é feriado se tem horario dando preferencia sempre pro dia e horario




                    }  
                       
                }   

            }

            return datesAppointments;
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


    }
}
