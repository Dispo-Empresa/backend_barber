using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispo.Barber.Domain.DTOs.Chat
{
    public class AvailableSlotRequestDto
    {
        public int Duration { get; set; }
        public long IdUser { get; set; }
        public DateTime DateTimeSchedule { get; set; }
    }

}
