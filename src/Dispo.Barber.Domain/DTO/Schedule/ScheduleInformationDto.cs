using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispo.Barber.Domain.DTO.Schedule
{
    public class ScheduleInformationDto
    {
        public List<string> DayOfWeek { get; set; }
        public List<string> StartDate { get; set; }
        public List<string> EndDate { get; set; }
        public List<bool> IsRest { get; set; }
        public List<bool> DayOff { get; set; }
    }
}
