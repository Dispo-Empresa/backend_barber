using Dispo.Barber.Domain.DTO.User;

namespace Dispo.Barber.Domain.DTO.BusinessUnity
{
    public class CreateBusinessUnityDTO
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string CEP { get; set; }
        public string Street { get; set; }
        public List<CreateUserDTO> Users { get; set; }
    }
}
