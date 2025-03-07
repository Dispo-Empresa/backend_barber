using System.ComponentModel;

namespace Dispo.Barber.Domain.Entities
{
    public class UserSchedule : EntityBase
    {
        public UserSchedule(DayOfWeek dayOfWeek, string start, string endDate, bool isRest, bool dayOff, bool enabled)
        {
            DayOfWeek = dayOfWeek;
            StartDate = start;
            EndDate = endDate;
            IsRest = isRest;
            DayOff = dayOff;
            Enabled = enabled;
        }

        public UserSchedule()
        {
        }

        public DayOfWeek DayOfWeek { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public DateTime? StartDay { get; set; }
        public DateTime? EndDay { get; set; }
        public bool IsRest { get; set; }
        public bool DayOff { get; set; }
        public bool Enabled { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }
    }
}
