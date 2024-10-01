using Dispo.Barber.Domain.Enum;

namespace Dispo.Barber.Domain.DTO.User
{
    public class UpdateUserDTO
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string Phone { get; set; }
        public UserRole Role { get; set; }
        public bool Active { get; set; } = true;
    }
}
