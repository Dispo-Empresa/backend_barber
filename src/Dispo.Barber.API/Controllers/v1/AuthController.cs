using Dispo.Barber.Application.AppServices.Interfaces;
using Dispo.Barber.Domain.DTOs.Authentication.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers.v1
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController(IAuthAppService authAppService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] AuthenticationRequest request, CancellationToken cancellationToken)
        {
            var jwt = await authAppService.AuthenticateAsync(request, cancellationToken);
            return Ok(jwt);
        }

        [AllowAnonymous]
        [HttpGet("refresh/{refreshToken}")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken, string refreshToken)
        {
            var jwt = await authAppService.RefreshAuthenticationToken(cancellationToken, refreshToken, Request.Headers.Authorization);
            return Ok(jwt);
        }
    }
}
