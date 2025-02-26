using Dispo.Barber.Domain.DTO.Company;

namespace Dispo.Barber.Domain.DTO.User
{
    public class CreateBarbershopSchemeDto
    {
        public CreateOwnerUserDTO OwnerUser { get; set; }
        public CreateCompanyDTO Company { get; set; }
    }
}
