using Dispo.Barber.Domain.Enums;

namespace Dispo.Barber.Domain.DTOs.User
{
    public class CreateEmployeeUserDTO
    {
        public string Phone { get; set; }
        public long BusinessUnityId { get; set; }
        public UserRole Role { get; set; }
        public List<long>? Services { get; set; }
    }
}
