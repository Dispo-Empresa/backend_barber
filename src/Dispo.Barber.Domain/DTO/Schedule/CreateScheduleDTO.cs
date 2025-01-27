using System.ComponentModel;

namespace Dispo.Barber.Domain.DTO.Schedule
{
    public class CreateScheduleDTO
    {
        public required DayOfWeek DayOfWeek { get; set; }

        [DefaultValue("08:00 | null")]
        public string? StartDate { get; set; }

        [DefaultValue("18:00 | null")]
        public string? EndDate { get; set; }

        //[DefaultValue("14/01/2025 | null")]
        public DateTime? StartDay { get; set; }

        //[DefaultValue("25/01/2025 | null")]
        public DateTime? EndDay { get; set; }

        public required bool IsRest { get; set; }
        public required bool DayOff { get; set; }
        public required long UserId { get; set; }
    }
}
