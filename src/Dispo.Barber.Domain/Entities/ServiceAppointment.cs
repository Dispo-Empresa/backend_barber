namespace Dispo.Barber.Domain.Entities
{
    public class ServiceAppointment : EntityBase
    {
        public long AppointmentId { get; set; }
        public Appointment Appointment { get; set; }

        public long ServiceId { get; set; }
        public Service Service { get; set; }
    }
}
