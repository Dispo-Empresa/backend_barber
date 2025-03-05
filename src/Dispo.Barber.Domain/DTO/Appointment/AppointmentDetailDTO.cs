using Dispo.Barber.Domain.DTO.Service;
using Dispo.Barber.Domain.Enums;

namespace Dispo.Barber.Domain.DTO.Appointment
{
    public class AppointmentDetailDTO
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? Customer { get; set; }

        public List<ServiceDetailDTO> Services { get; set; } = [];
    }
}
