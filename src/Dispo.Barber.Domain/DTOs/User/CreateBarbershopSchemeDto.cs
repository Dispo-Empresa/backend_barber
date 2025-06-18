using Dispo.Barber.Domain.DTOs.Company;
using Dispo.Barber.Domain.Enums;

namespace Dispo.Barber.Domain.DTOs.User
{
    public class CreateBarbershopSchemeDto
    {
        public CreateOwnerUserDTO OwnerUser { get; set; }
        public CreateCompanyDTO Company { get; set; }
        public LicensePlan PlanType { get; set; }
    }
}
