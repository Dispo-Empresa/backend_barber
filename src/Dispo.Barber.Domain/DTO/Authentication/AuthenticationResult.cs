namespace Dispo.Barber.Domain.DTO.Authentication
{
    public class AuthenticationResult
    {
        public AuthenticationResult(string token, Entities.User user)
        {
            Token = token;
            UserId = user.Id;
            CompanyId = user.BusinessUnity?.CompanyId;
            BusinessUnityId = user.BusinessUnityId;
            Phone = user.Phone;
            CompanyName = user.BusinessUnity.Company.Name;
            CompanyPhone = user.BusinessUnity.Phone;
            Name = user.Name;
            Slug = user.EntireSlug();
        }

        public AuthenticationResult()
        {
        }

        public string Token { get; set; }
        public long UserId { get; set; }
        public long? CompanyId { get; set; }
        public long? BusinessUnityId { get; set; }
        public string Phone { get; set; }
        public string CompanyName { get; set; }
        public string CompanyPhone { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
    }
}
