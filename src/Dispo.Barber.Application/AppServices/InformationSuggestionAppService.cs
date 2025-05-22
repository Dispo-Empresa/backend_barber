using AutoMapper;
using Dispo.Barber.Application.AppServices.Interface;
using Dispo.Barber.Domain.DTOs.Appointment;
using Dispo.Barber.Domain.DTOs.Chat;
using Dispo.Barber.Domain.DTOs.Customer;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Domain.Utils;


namespace Dispo.Barber.Application.AppServices
{
    public class InformationSuggestionAppService(IUnitOfWork unitOfWork, IMapper mapper, IAppointmentAppService appointmentAppService, IUserRepository userRepository) : IInformationSuggestionAppService
    {

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
                    .Take(5) 
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
                            .Select(a => new { a.Date, a.Date.DayOfWeek, a.Customer }) 
                            .ToList();

                            var allCustomerHourAppointments = appointment.Customer.Appointments
                            .OrderBy(a => a.Date) 
                            .Select(a => new { a.Date, a.Date.Hour, a.Date.Minute }) 
                            .ToList();

                            var groupedDays = allCustomerAppointments
                            .GroupBy(a => a.DayOfWeek)
                            .OrderByDescending(g => g.Count()) 
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

                            var referenceDateOfWeek = referenceDate.AddDays(averageInterval); 

                            if (referenceDateOfWeek.DayOfWeek == DayOfWeek.Sunday)
                            {
                                referenceDateOfWeek.AddDays(1);
                            }

                            if (referenceDateOfWeek.DayOfWeek != mostFrequentDay.Last().DayOfWeek)
                            {
                              
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
                            .GroupBy(a => a.AcceptedUser) 
                            .Select(group => new
                            {
                                Barber = group.Key,
                                TotalAppointments = group.Count() 
                            })
                            .OrderByDescending(barber => barber.TotalAppointments) 
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


                            var availableSlots = await GetAvailableSlotsInternalSuggestedAsync(cancellationTokenSource.Token, availableSlotRequestDto);

                            var formtHour = adjustedReferenceDate.TimeOfDay;

                            var serviceDuration = TimeSpan.FromMinutes(topServiceAppointments.Service.Service.Duration);
                            var endHour = formtHour + serviceDuration;

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

                                await appointmentAppService.CreateAsync(cancellationTokenSource.Token, createAppointmentDTO);

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

        private int SumDurationService(List<ServiceAppointment> serviceAppointments)
        {
            int duration = 0;
            foreach (var serviceAppointment in serviceAppointments)
            {
                duration += serviceAppointment.Service.Duration;
            }
            return duration;
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

        private Dictionary<string, List<TimeSpan>> InitializeAvailableSuggestedSlots()
        {
            return new Dictionary<string, List<TimeSpan>>
            {
                { "morning", new List<TimeSpan>() },
                { "afternoon", new List<TimeSpan>() },
                { "evening", new List<TimeSpan>() }
            };
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

        private List<DateTime> GetTimeIntervals(int duration, List<UserSchedule> userSchedules, List<UserSchedule> breaks, List<UserSchedule> dayOffs, DateTime selectedDate, bool dayIsEqual)
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

    }
}
