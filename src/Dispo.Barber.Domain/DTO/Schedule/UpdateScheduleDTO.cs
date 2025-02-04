using System.ComponentModel;

namespace Dispo.Barber.Domain.DTO.Schedule
{
    public class UpdateScheduleDTO
    {
        public DayOfWeek? DayOfWeek { get; set; }

        [DefaultValue("08:00 | null")]
        public string? StartDate { get; set; }

        [DefaultValue("18:00 | null")]
        public string? EndDate { get; set; }

        public DateTime? StartDay { get; set; }

        public DateTime? EndDay { get; set; }

        public bool? IsRest { get; set; }
        public bool? DayOff { get; set; }
        public bool? Enabled { get; set; }
    }
}
