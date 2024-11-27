using Dispo.Barber.Domain.DTO.Service;
using Dispo.Barber.Domain.Enum;

namespace Dispo.Barber.Domain.DTO.Appointment
{
    public class AppointmentDetailDTO
    {
        public long Id { get; set; }
        public DateTime Data { get; set; }
        public AppointmentStatus Status { get; set; }

        public List<ServiceDetailDTO> Services { get; set; } = [];
    }
}
