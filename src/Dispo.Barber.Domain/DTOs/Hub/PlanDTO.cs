using Dispo.Barber.Domain.Enums;

namespace Dispo.Barber.Domain.DTOs.Hub
{
    public class PlanDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public double UserLimit { get; set; }

        public bool IsFreePlan() => Id == (int)PlanType.BarberFree;
        public bool IsPremiumPlan() => Id == (int)PlanType.BarberPremium;
        public bool IsTrial() => Id == (int)PlanType.BarberPremiumTrial;
    }
}
