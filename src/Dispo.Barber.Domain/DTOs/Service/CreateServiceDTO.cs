using Dispo.Barber.Domain.Enums;

namespace Dispo.Barber.Domain.DTOs.Service
{
    public class CreateServiceDTO
    {
        public long CompanyId { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Duration { get; set; }
        public ServiceStatus Status { get; set; }
    }
}
