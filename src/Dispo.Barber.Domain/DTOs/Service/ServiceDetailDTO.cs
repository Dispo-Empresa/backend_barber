namespace Dispo.Barber.Domain.DTOs.Service
{
    public class ServiceDetailDTO
    {
        public required long Id { get; set; }
        public required string Description { get; set; }
        public int Realized { get; set; }
        public decimal RealizedPercentage { get; set; }
    }
}
