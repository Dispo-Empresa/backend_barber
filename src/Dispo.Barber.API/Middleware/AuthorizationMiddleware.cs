using System.Net;
using Microsoft.Extensions.Caching.Memory;

namespace Dispo.Barber.API.Middleware
{
    public class AuthorizationMiddleware
    {
        private const string BlacklistedJwtKey = "blacklisted-{0}";

        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IMemoryCache cache)
        {
            if (!string.IsNullOrEmpty(context.Request.Headers.Authorization))
            {
                if (cache.Get<bool>(string.Format(BlacklistedJwtKey, context.Request.Headers.Authorization)))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.CompleteAsync();
                    return;
                }
            }

            await _next(context);
        }

        private string BuildBlacklistedJWTKey(string token)
        {
            return string.Format(BlacklistedJwtKey, token);
        }
    }
}
