using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispo.Barber.Application.AppServices.Interface
{
    public interface InformationSuggestionAppService
    {
        Task<bool> GetSuggestionAppointmentAsync();
    }
}
