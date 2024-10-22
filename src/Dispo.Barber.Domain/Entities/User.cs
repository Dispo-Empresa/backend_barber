using Dispo.Barber.Domain.Enum;

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

        public long? BusinessUnityId { get; set; }
        public BusinessUnity? BusinessUnity { get; set; }

        public IList<ServiceUser> ServicesUser { get; set; } = new List<ServiceUser>();
        public IList<Appointment> Appointments { get; set; } = new List<Appointment>();
        public IList<UserSchedule> Schedules { get; set; } = new List<UserSchedule>();

        public string EstimatedGains()
        {
            if (Appointments.Count == 0)
            {
                return "N/A";
            }

            return $"R${Appointments.Where(w => w.Date >= DateTime.Today && w.Date <= DateTime.Today.AddDays(1).AddTicks(-1)).Sum(s => s.Services.Select(s => s.Service).Sum(s => s.Price))}";
        }

        public string TodayAppointments()
        {
            if (Appointments.Count == 0)
            {
                return "N/A";
            }

            return $"{Appointments.Count(w => w.Date >= DateTime.Today && w.Date <= DateTime.Now)}/{Appointments.Count(w => w.Date >= DateTime.Today && w.Date <= DateTime.Today.AddDays(1).AddTicks(-1))}";
        }

        public string ScheduledHours()
        {
            if (Appointments.Count == 0)
            {
                return "N/A";
            }

            return $"{FormatMinutesToHours(Appointments.Where(w => w.Date >= DateTime.Today && w.Date <= DateTime.Today.AddDays(1).AddTicks(-1)).Sum(s => s.Services.Select(s => s.Service).Sum(ss => ss.Duration)))}";
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
    }
}
