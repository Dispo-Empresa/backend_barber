namespace Dispo.Barber.Domain.DTO.Schedule
{
    public class CreateScheduleDTO
    {
        public required DayOfWeek DayOfWeek { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required bool IsRest { get; set; }
        public required bool DayOff { get; set; }
        public required long UserId { get; set; }
    }
}
