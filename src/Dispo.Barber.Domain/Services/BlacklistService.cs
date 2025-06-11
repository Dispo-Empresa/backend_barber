using Dispo.Barber.Domain.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Dispo.Barber.Domain.Services
{
    public class BlacklistService(IMemoryCache cache) : IBlacklistService
    {
        private const string BlacklistedJwtKey = "blacklisted-{0}";

        public bool IsBlacklisted(object token)
        {
            return cache.Get<bool>(BuildBlacklistedJWTKey(token));
        }

        public bool PutInBlacklist(object token)
        {
            return cache.Set(BuildBlacklistedJWTKey(token), true);
        }

        private string BuildBlacklistedJWTKey(object token)
        {
            return string.Format(BlacklistedJwtKey, token);
        }
    }
}
