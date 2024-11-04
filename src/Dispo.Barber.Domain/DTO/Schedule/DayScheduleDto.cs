using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispo.Barber.Domain.DTO.Schedule
{
    public class DayScheduleDto
    {
        public string DayOfWeek { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool IsRest { get; set; }
        public bool DayOff { get; set; }
    }
}
