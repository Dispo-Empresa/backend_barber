using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dispo.Barber.Application.AppService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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
            var jwt = await authAppService.Authenticate(cancellationToken, phone, password);
            return Ok(jwt);
        }
    }
}
