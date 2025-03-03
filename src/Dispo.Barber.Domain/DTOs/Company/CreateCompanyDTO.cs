using Dispo.Barber.Domain.DTOs.Service;

namespace Dispo.Barber.Domain.DTOs.Company
{
    public class CreateCompanyDTO
    {
        public string Name { get; set; }
        //public string Phone { get; set; }
        //public string Logo { get; set; }

        //public List<CreateBusinessUnityDTO>? BusinessUnities { get; set; }
        public List<CreateServiceDTO>? Services { get; set; }
    }
}
