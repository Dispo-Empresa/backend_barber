namespace Dispo.Barber.Domain.Entities
{
    public class Token : EntityBase
    {
        public required string RefreshToken { get; set; }
        public required DateTime ExpirationDate { get; set; }

        public required long UserId { get; set; }
        public User? User { get; set; }
    }
}
