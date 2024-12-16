using Dispo.Barber.API.Hubs;
using Dispo.Barber.Application.AppService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController(IAuthAppService authAppService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken, string phone, string password)
        {
            var jwt = await authAppService.AuthenticateAsync(cancellationToken, phone, password);
            return Ok(jwt);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> LancarNotificacao()
        {
            await new NotificationHub().SendNotificationAsync(
                                        "dVawZ8UZRzOU3ciGgrOLEX:APA91bGm7TXLvOVzTC6XsvjiH7naPXoFi29AcZ5JtqIVUgMKyLEzx4b7PKzpL67O9gGGFCAydacjwZEV0OwseEO7iToETiFHqZP2zYUIRtZbLHxufLwcK7Q",
                                        "Novo agendamento",
                                        "Matheus fez um novo agendamento para o dia 17/01"
                                    );

            return Ok();
        }
    }
}
