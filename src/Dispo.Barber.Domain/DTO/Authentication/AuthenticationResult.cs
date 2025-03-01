using Dispo.Barber.Domain.Enum;

namespace Dispo.Barber.Domain.DTO.Authentication
{
    public class AuthenticationResult
    {
        public AuthenticationResult(string token, string refreshToken, Entities.User user)
        {
            Token = token;
            RefreshToken = refreshToken;
            UserId = user.Id;
            CompanyId = user.BusinessUnity?.CompanyId;
            BusinessUnityId = user.BusinessUnityId;
            Phone = user.Phone;
            CompanyName = user.BusinessUnity.Company.Name;
            CompanyPhone = user.BusinessUnity.Phone;
            Name = user.Name;
            Slug = user.EntireSlug();
            Photo = user.Photo;
            DeviceToken = user.DeviceToken;
            Role = user.Role;
        }

        public AuthenticationResult()
        {
        }

        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public long UserId { get; set; }
        public long? CompanyId { get; set; }
        public long? BusinessUnityId { get; set; }
        public string Phone { get; set; }
        public string CompanyName { get; set; }
        public string CompanyPhone { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public byte[]? Photo { get; set; }
        public string DeviceToken { get; set; }
        public UserRole Role { get; set; }
    }
}
