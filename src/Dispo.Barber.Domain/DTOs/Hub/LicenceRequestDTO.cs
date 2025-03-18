using Dispo.Barber.Domain.Enums;

namespace Dispo.Barber.Domain.DTOs.Hub
{
    public class LicenceRequestDTO
    {
        public long CompanyId { get; set; }
        public PlanType PlanType { get; set; }
        public DateTime? Expiration { get; set; }
    }
}
