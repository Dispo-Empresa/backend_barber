namespace Dispo.Barber.Domain.DTOs.Schedule
{
    public class ScheduleDTO
    {
        public required string StartDate { get; set; }
        public required string EndDate { get; set; }
        public required DayOfWeek DayOfWeek { get; set; }
        public bool DayOff { get; set; }
    }
}
