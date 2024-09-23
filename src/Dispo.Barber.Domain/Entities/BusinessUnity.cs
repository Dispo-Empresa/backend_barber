namespace Dispo.Barber.Domain.Entities
{
    public class BusinessUnity : EntityBase
    {
        public string? CNPJ { get; set; }
        public string? Phone { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string CEP { get; set; }
        public string Street { get; set; }
        public string? Number { get; set; }
        public string? Complement { get; set; }

        public long CompanyId { get; set; }
        public Company Company { get; set; }

        public List<User> Users { get; set; } = new List<User>();
    }
}
