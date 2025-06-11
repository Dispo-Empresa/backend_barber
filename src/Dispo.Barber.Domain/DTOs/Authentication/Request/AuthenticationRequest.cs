using Dispo.Barber.Domain.Enums;

namespace Dispo.Barber.Domain.DTOs.Authentication.Request
{
    public class AuthenticationRequest
    {
        public string Phone { get; set; }
        public string Password { get; set; }
        public DevicePlatform Platform { get; set; }
        public string DeviceToken { get; set; }
    }
}