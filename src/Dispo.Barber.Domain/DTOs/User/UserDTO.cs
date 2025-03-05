using Dispo.Barber.Domain.Enums;

namespace Dispo.Barber.Domain.DTOs.User
{
    public class UserDTO
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public byte[]? Photo { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public UserStatus Status { get; set; }
        public string Link { get; set; }
        public UserRole Role { get; set; }
    }
}
