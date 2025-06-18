using Dispo.Barber.Domain.Enums;

namespace Dispo.Barber.Domain.DTOs.Hub
{
    public class PlanData
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public double UserLimit { get; set; }

        public bool IsFreePlan() => Id == (int)LicensePlan.BarberFree;
        public bool IsPremiumPlan() => Id == (int)LicensePlan.BarberPremium;
        public bool IsTrial() => Id == (int)LicensePlan.BarberPremiumTrial;
    }
}
