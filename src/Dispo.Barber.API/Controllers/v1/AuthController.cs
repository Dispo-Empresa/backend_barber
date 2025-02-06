using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Twilio.Http;

namespace Dispo.Barber.API.Controllers.v1
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController(IAuthAppService authAppService, INotificationService notificationService) : ControllerBase
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

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> LancarNotificacao(CancellationToken cancellationToken)
        {
            await notificationService.NotifyAsync(cancellationToken,
                                                  "dVawZ8UZRzOU3ciGgrOLEX:APA91bGm7TXLvOVzTC6XsvjiH7naPXoFi29AcZ5JtqIVUgMKyLEzx4b7PKzpL67O9gGGFCAydacjwZEV0OwseEO7iToETiFHqZP2zYUIRtZbLHxufLwcK7Q",
                                                  "Novo agendamento",
                                                  "Matheus fez um novo agendamento para o dia 17/01");

            return Ok();
        }
    }
}
