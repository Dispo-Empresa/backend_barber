using Dispo.Barber.Domain.Utils;

namespace Dispo.Barber.Domain.DTOs.Hub
{
    public class LicenseDTO
    {
        public DateTime ExpirationDate { get; set; }
        public PlanDTO Plan { get; set; }

        public bool IsExpired()
        {
            return LocalTime.Now >= ExpirationDate;
        }
    }
}
