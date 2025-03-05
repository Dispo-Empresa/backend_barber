namespace Dispo.Barber.Domain.DTO.Hub
{
    public class PlanDTO
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public double UserLimit { get; set; }
        public decimal AdditionalPricePerUser { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public long Id { get; set; }
    }
}
