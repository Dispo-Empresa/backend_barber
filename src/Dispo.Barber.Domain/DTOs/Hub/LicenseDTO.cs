using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Utils;

namespace Dispo.Barber.Domain.DTOs.Hub
{
    public class LicenseDTO
    {
        public string ExpirationDate { get; set; }
        public PlanDTO Plan { get; set; }

        public bool IsExpired()
        {
            var expiration = DateTime.Parse(ExpirationDate);
            return LocalTime.Now >= expiration;
        }
    }
}
