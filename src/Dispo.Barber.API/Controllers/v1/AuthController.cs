using Dispo.Barber.Application.AppServices.Interface;
using Dispo.Barber.Domain.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers.v1
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController(IAuthAppService authAppService, INotificationSenderProvider notificationService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken, string phone, string password)
        {
            var jwt = await authAppService.AuthenticateAsync(cancellationToken, phone, password);
            return Ok(jwt);
        }

        [AllowAnonymous]
        [HttpGet("refresh/{refreshToken}")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken, string refreshToken)
        {
            var jwt = await authAppService.RefreshAuthenticationToken(cancellationToken, refreshToken, Request.Headers.Authorization);
            return Ok(jwt);
        }

        [Authorize]
        [HttpPatch("{userId}/purchase-token/{purchaseToken}")]
        public async Task<IActionResult> UpdatePurchaseToken([FromRoute] int userId, [FromRoute] string purchaseToken, CancellationToken cancellationToken)
        {
            await authAppService.UpdatePurchaseTokenTeste(userId, purchaseToken, cancellationToken);
            return Ok();
        }
    }
}
