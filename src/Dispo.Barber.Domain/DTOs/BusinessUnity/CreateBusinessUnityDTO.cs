namespace Dispo.Barber.Domain.DTOs.BusinessUnity
{
    public class CreateBusinessUnityDTO
    {
        public long CompanyId { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string CEP { get; set; }
        public string Street { get; set; }
    }
}
