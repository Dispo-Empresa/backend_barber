namespace Dispo.Barber.Domain.Entities
{
    public class Appointment : EntityBase
    {
        public DateTime Date { get; set; }
        public DateTime AccomplishedDate { get; set; }
        public string? CustomerObservation { get; set; }
        public string? AcceptedUserObservation { get; set; }

        public required long ServiceId { get; set; }
        public required Service Service { get; set; }

        public required long BusinessUnityId { get; set; }
        public required BusinessUnity BusinessUnity { get; set; }

        public required long CustomerId { get; set; }
        public required Customer Customer { get; set; }

        public long? AcceptedUserId { get; set; }
        public User? AcceptedUser { get; set; }
    }
}
