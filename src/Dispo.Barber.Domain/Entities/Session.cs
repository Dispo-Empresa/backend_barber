namespace Dispo.Barber.Domain.Entities
{
    public class Session : EntityBase
    {
        public long UserId { get; set; }
        public string ConfirmationCode { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
