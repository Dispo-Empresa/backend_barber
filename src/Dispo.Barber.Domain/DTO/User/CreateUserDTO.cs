using Dispo.Barber.Domain.Enum;

namespace Dispo.Barber.Domain.DTO.User
{
    public class CreateUserDTO
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string Phone { get; set; }
        public UserRole Role { get; set; }
        public UserStatus Status { get; set; }
        public bool Active { get; set; } = true;
        public string DeviceToken { get; set; }
        public List<long> Services { get; set; }
    }
}
