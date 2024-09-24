namespace Dispo.Barber.Domain.Entities
{
    public class Customer : EntityBase
    {
        public string Name { get; set; }
        public string Phone { get; set; }

        public List<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
