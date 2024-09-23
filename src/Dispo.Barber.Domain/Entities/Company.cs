namespace Dispo.Barber.Domain.Entities
{
    public class Company : EntityBase
    {
        public string Name { get; set; }
        public string Logo { get; set; }

        public IList<ServiceCompany> ServicesCompany { get; set; }
        public IList<BusinessUnity> BusinessUnities { get; set; }
    }
}
