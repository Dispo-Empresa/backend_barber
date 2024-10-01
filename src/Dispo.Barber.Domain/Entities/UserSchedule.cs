namespace Dispo.Barber.Domain.Entities
{
    public class UserSchedule : EntityBase
    {
        public UserSchedule(DayOfWeek dayOfWeek, string start, string endDate, bool isRest, bool dayOff)
        {
            DayOfWeek = dayOfWeek;
            StartDate = start;
            EndDate = endDate;
            IsRest = isRest;
            DayOff = dayOff;
        }

        public UserSchedule()
        {
        }

        public DayOfWeek DayOfWeek { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool IsRest { get; set; }
        public bool DayOff { get; set; }
    }
}
