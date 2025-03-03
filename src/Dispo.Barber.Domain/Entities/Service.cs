using Dispo.Barber.Domain.Enums;

namespace Dispo.Barber.Domain.Entities
{
    public class Service : EntityBase
    {
        public string Description { get; set; }
        public double Price { get; set; }
        public int Duration { get; set; }
        public ServiceStatus Status { get; set; }

        public IList<ServiceCompany> CompanyServices { get; set; }
        public IList<ServiceUser> UserServices { get; set; }
    }
}