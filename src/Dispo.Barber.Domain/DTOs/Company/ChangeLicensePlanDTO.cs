using Dispo.Barber.Domain.Enums;

namespace Dispo.Barber.Domain.DTOs.Company
{
    public class ChangeLicensePlanDTO
    {
        public LicensePlan LicensePlan { get; set; }
        public string? PurchaseToken { get; set; }
    }
}
