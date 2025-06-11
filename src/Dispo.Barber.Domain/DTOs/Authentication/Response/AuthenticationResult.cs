using Dispo.Barber.Domain.DTOs.Hub;
using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Integration.SubscriptionClient.Models;

namespace Dispo.Barber.Domain.DTOs.Authentication.Response
{
    public class AuthenticationResult
    {
        public AuthenticationResult(string token, string refreshToken, Entities.User user, SubscriptionData barbershopData)
        {
            Token = token;
            RefreshToken = refreshToken;
            User = new UserData
            {
                UserId = user.Id,
                Phone = user.Phone,
                Name = user.Name,
                ScheduleLink = user.EntireSlug(),
                Photo = user.Photo,
                DeviceToken = user.DeviceToken,
                Role = user.Role,
                Barbershop = new BarbershopData
                {
                    CompanyId = user.BusinessUnity?.CompanyId ?? 0,
                    BusinessUnityId = user.BusinessUnity?.Id ?? 0,
                    Name = user.BusinessUnity?.Company.Name ?? string.Empty,
                    ScheduleLink = user.BusinessUnity?.EntireSlug() ?? string.Empty,
                    OwnerId = user.BusinessUnity?.Company.OwnerId ?? 0
                }
            };
            Licence = barbershopData;
        }

        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public UserData User { get; set; }
        public SubscriptionData Licence { get; set; }
    }

    public class UserData
    {
        public long UserId { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string ScheduleLink { get; set; }
        public byte[]? Photo { get; set; }
        public string DeviceToken { get; set; }
        public UserRole Role { get; set; }
        public BarbershopData Barbershop { get; set; }
    }

    public class BarbershopData
    {
        public long CompanyId { get; set; }
        public long BusinessUnityId { get; set; }
        public string Name { get; set; }
        public string ScheduleLink { get; set; }
        public long OwnerId { get; set; }
    }

    public class SubscriptionData
    {
        public DateTime ExpirationDate { get; set; }
        public SubscriptionStatus Status { get; set; }
        public bool HasTrialExpiredError { get; set; }
        public bool HasChangedPlataformError { get; set; }
        public DevicePlatform Platform { get; set; }
        public PlanDTO Plan { get; set; }

        public bool IsSubscriptionValid => Status is SubscriptionStatus.Active or SubscriptionStatus.InGracePeriod;
    }
}