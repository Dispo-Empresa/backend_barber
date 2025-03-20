namespace Dispo.Barber.Domain.DTOs.Hub
{
    public class LicenceDTO
    {
        public string Key { get; set; }
        public DateTime ExpirationDate { get; set; }
        public PlanDTO Plan { get; set; }
    }
}
