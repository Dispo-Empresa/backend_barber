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
        }

        public AuthenticationResult()
        {
        }

        public string Token { get; set; }
        public long UserId { get; set; }
        public long? CompanyId { get; set; }
        public long? BusinessUnityId { get; set; }
        public string Phone { get; set; }
    }
}
