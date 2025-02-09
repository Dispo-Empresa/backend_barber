using Dispo.Barber.Domain.Enum;

namespace Dispo.Barber.Domain.DTO.User
{
    public class UserDTO
    {
        public long Id { get; set; }
        public byte[]? Photo { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public UserStatus Status { get; set; }
        public string Link { get; set; }
    }
}
