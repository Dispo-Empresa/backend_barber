using Dispo.Barber.Domain.DTOs.Service;
using Dispo.Barber.Domain.Enums;

namespace Dispo.Barber.Domain.DTOs.Appointment
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
