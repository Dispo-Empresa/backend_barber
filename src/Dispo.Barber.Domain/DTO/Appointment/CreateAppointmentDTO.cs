using Dispo.Barber.Domain.DTO.Customer;

namespace Dispo.Barber.Domain.DTO.Appointment
{
    public class CreateAppointmentDTO
    {
        public DateTime Date { get; set; }
        public string? CustomerObservation { get; set; }
        public string? AcceptedUserObservation { get; set; }
        public long? AcceptedUserId { get; set; }

        public required long BusinessUnityId { get; set; }
        public required long ServiceId { get; set; }

        public CustomerDTO Customer { get; set; }
    }
}
