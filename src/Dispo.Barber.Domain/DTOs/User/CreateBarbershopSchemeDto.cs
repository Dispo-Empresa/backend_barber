using Dispo.Barber.Domain.DTOs.Company;

namespace Dispo.Barber.Domain.DTOs.User
{
    public class CreateBarbershopSchemeDto
    {
        public CreateOwnerUserDTO OwnerUser { get; set; }
        public CreateCompanyDTO Company { get; set; }
    }
}
