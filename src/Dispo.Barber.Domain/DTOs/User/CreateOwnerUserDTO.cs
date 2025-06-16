using Dispo.Barber.Domain.Enums;

namespace Dispo.Barber.Domain.DTOs.User
{
    public class CreateOwnerUserDTO
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public byte[]? Photo { get; set; }
        public string DeviceToken { get; set; }
        public DevicePlatform Platform { get; set; }
    }
}