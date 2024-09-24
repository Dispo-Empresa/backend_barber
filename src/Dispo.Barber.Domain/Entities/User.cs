using Dispo.Barber.Domain.Enum;

namespace Dispo.Barber.Domain.Entities
{
    public class User : EntityBase
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string Phone { get; set; }
        public UserRole Role { get; set; }
        public bool Active { get; set; }

        public long? BusinessUnityId { get; set; }
        public BusinessUnity? BusinessUnity { get; set; }

        public IList<ServiceUser> ServicesUser { get; set; } = new List<ServiceUser>();
        public IList<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
