using System.Net;
using Dispo.Barber.Domain.Services.Interface;

namespace Dispo.Barber.API.Middleware
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IBlacklistService blacklistService)
        {
            if (!string.IsNullOrEmpty(context.Request.Headers.Authorization))
            {
                if (IsBlacklisted(context, blacklistService))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.CompleteAsync();
                    return;
                }
            }

            await _next(context);
        }

        private bool IsBlacklisted(HttpContext context, IBlacklistService blacklistService)
        {
            if (blacklistService.IsBlacklisted(context.Request.Headers.Authorization)) {
                return true;
            }

            var userId = GetUserId(context);
            if (string.IsNullOrEmpty(userId)) {
                return false;
            }

            return blacklistService.IsBlacklisted(userId);
        }

        private string GetUserId(HttpContext context)
        {
            var claimId = context.User.FindFirst("id");
            if (claimId is null)
            {
                return "";
            }

            return claimId.Value;
        }
    }
}
