using Dispo.Barber.Domain.Enum;

namespace Dispo.Barber.Domain.DTO.User
{
    public class CreateEmployeeUserDTO
    {
        public string Phone { get; set; }
        public UserRole Role { get; set; }
        public List<long>? Services { get; set; }
    }
}
