using Dispo.Barber.Domain.Utils;

namespace Dispo.Barber.Domain.DTOs.Hub
{
    public class LicenseDTO
    {
        public DateTime ExpirationDate { get; set; }
        public PlanData Plan { get; set; }

        public bool IsExpired()
        {
            return !Plan.IsFreePlan() && LocalTime.Now >= ExpirationDate;
        }
    }
}
