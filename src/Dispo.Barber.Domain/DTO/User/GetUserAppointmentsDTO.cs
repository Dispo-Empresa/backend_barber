using System.Text.Json.Serialization;
using Dispo.Barber.Domain.Enum;
using Newtonsoft.Json.Converters;

namespace Dispo.Barber.Domain.DTO.User
{
    public class GetUserAppointmentsDTO
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public AppointmentStatus? Status { get; set; }
    }
}
