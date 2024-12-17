namespace Dispo.Barber.Domain.Entities
{
    public class ServiceUser : EntityBase
    {
        public required long UserId { get; set; }
        public User User { get; set; }

        public required long ServiceId { get; set; }
        public Service Service { get; set; }

        public DateTime? ProvidesUntil { get; set; }
    }
}
