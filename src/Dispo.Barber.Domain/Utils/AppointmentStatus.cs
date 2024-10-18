using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispo.Barber.Domain.Utils
{
    public enum AppointmentStatus
    {
        Scheduled,       // Agendado
        InProgress,      // Em andamento
        Completed,       // Concluído
        Canceled         // Cancelado
    }

}
