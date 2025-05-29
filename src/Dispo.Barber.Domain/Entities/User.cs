using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Utils;
using Dispo.Barber.Domain.Utils.Constants;

namespace Dispo.Barber.Domain.Entities
{
    public class User : EntityBase
    {
        public string Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string Phone { get; set; }
        public UserRole Role { get; set; }
        public UserStatus Status { get; set; }
        public string Slug { get; set; }
        public byte[]? Photo { get; set; }
        public string DeviceToken { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UnreadNotificationsCount { get; set; }

        public long? BusinessUnityId { get; set; }
        public BusinessUnity? BusinessUnity { get; set; }

        public Token? RefreshToken { get; set; }

        public IList<ServiceUser> ServicesUser { get; set; } = [];
        public IList<Appointment> Appointments { get; set; } = [];
        public IList<UserSchedule> Schedules { get; set; } = [];

        public string EstimatedGains()
        {
            if (Appointments.Count == 0)
            {
                return "N/A";
            }

            return $"R${Appointments.Where(w => (w.Status == AppointmentStatus.Scheduled || w.Status == AppointmentStatus.Completed) && w.Date >= DateTime.Today && w.Date <= DateTime.Today.AddDays(1).AddTicks(-1)).Sum(s => s.Services.Select(s => s.Service).Sum(s => s.Price))}";
        }

        public string TodayAppointments()
        {
            if (Appointments.Count == 0)
            {
                return "N/A";
            }

            return $"{Appointments.Where(w => w.Status == AppointmentStatus.Completed)
                                  .Count(w => w.Date >= DateTime.Today && w.Date <= LocalTime.Now)}/{Appointments.Where(w => (w.Status == AppointmentStatus.Scheduled || w.Status == AppointmentStatus.Completed))
                                                                                                                 .Count(w => w.Date >= DateTime.Today && w.Date <= DateTime.Today.AddDays(1).AddTicks(-1))}";
        }

        public string ScheduledHours()
        {
            if (Appointments.Count == 0)
            {
                return "N/A";
            }

            return $"{FormatMinutesToHours(Appointments.Where(w => w.Status == AppointmentStatus.Scheduled && w.Date >= DateTime.Today && w.Date <= DateTime.Today.AddDays(1).AddTicks(-1)).Sum(s => s.Services.Select(s => s.Service).Sum(ss => ss.Duration)))}";
        }

        public string ChairUsage()
        {
            if (Appointments.Count == 0)
            {
                return "N/A";
            }

            var restingHours = Schedules.Where(w => w.DayOfWeek == DateTime.Today.DayOfWeek && w.IsRest && (!string.IsNullOrEmpty(w.EndDate) && !string.IsNullOrEmpty(w.StartDate)))
                                        .Select(s => GetDifference(s.EndDate, s.StartDate)).ToList();
            var summedRestingHours = BulkSumDates(restingHours);

            var workingHours = Schedules.Where(w => w.DayOfWeek == DateTime.Today.DayOfWeek && !w.IsRest && (!string.IsNullOrEmpty(w.EndDate) && !string.IsNullOrEmpty(w.StartDate)))
                                        .Select(s => GetDifference(s.EndDate, s.StartDate)).ToList();

            var summedWorkingHours = BulkSumDates(workingHours);

            var totalAppointmentDuration = Appointments.Where(w => (w.Status == AppointmentStatus.Scheduled || w.Status == AppointmentStatus.Completed) && w.Date >= DateTime.Today && w.Date <= DateTime.Today.AddDays(1).AddTicks(-1)).Select(s => s.Services.Sum(s => s.Service.Duration)).Sum();
            var durationTimeSpan = TimeSpan.FromMinutes(totalAppointmentDuration);
            var hourMinute = durationTimeSpan.ToString(@"hh\:mm");
            return $"{Convert.ToInt32(hourMinute.Replace(":", "")) * 100 / Convert.ToInt32(GetDifference(summedWorkingHours, summedRestingHours).Replace(":", ""))}%";
        }

        private static string FormatMinutesToHours(int minutes)
        {
            int hours = minutes / 60;
            int remainingMinutes = minutes % 60;

            return $"{hours:D2}h{remainingMinutes:D2}m";
        }

        public bool IsPending()
        {
            return Status == UserStatus.Pending;
        }

        private string GetDifference(string endDate, string startDate)
        {
            var startTime = TimeSpan.Parse(startDate);
            var endTime = TimeSpan.Parse(endDate);
            var difference = startTime - endTime;
            return difference.ToString(@"hh\:mm");
        }

        private string SumDates(string firstDate, string secondDate)
        {
            var startTime = TimeSpan.Parse(firstDate);
            var endTime = TimeSpan.Parse(secondDate);
            var difference = startTime + endTime;
            return difference.ToString(@"hh\:mm");
        }

        private string BulkSumDates(List<string> dates)
        {
            var summedHours = "00:00";
            foreach (var workingHour in dates)
            {
                summedHours = SumDates(workingHour, summedHours);
            }
            return summedHours;
        }

        public string EntireSlug()
        {
            var barberIdEncripted = CryptoHelper.Encrypt(Id.ToString());
            return $"{Links.AuraChatBarberLink}{barberIdEncripted}";
        }
    }
}
