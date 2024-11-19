using Dispo.Barber.API.Hubs;
using Dispo.Barber.Application.AppService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

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
    }
}
