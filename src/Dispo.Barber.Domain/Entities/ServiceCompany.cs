namespace Dispo.Barber.Domain.Entities
{
    public class ServiceCompany : EntityBase
    {
        public long CompanyId { get; set; }
        public Company Company { get; set; }

        public long ServiceId { get; set; }
        public Service Service { get; set; }
    }
}
