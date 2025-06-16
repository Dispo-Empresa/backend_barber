using Dispo.Barber.Application.AppServices.Interfaces;
using Dispo.Barber.Domain.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers.v1
{
    [Authorize]
    [Route("api/v1/user-registrations")]
    [ApiController]
    public class UsersRegistrationController(IUserAppService userAppService) : ControllerBase
    {
        [HttpPost("create-employee-user")]
        public async Task<IActionResult> CreateEmployeeUserAsync([FromBody] CreateEmployeeUserDTO createEmployeeUser, CancellationToken cancellationToken)
        {
            await userAppService.CreateEmployeeUserAsync(cancellationToken, createEmployeeUser);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPatch("{id}/finalize-employee-user")]
        public async Task<IActionResult> FinalizeEmployeeUserRegistrationAsync([FromRoute] long id, [FromBody] FinalizeEmployeeUserDTO finalizeEmployeeUserDto, CancellationToken cancellationToken)
        {
            await userAppService.FinalizeEmployeeUserRegistrationAsync(cancellationToken, id, finalizeEmployeeUserDto);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("barbershop-scheme")]
        public async Task<IActionResult> CreateBarbershopSchemeAsync([FromBody] CreateBarbershopSchemeDto createBarbershopSchemeDto, CancellationToken cancellationToken)
        {
            await userAppService.CreateBarbershopSchemeAsync(cancellationToken, createBarbershopSchemeDto);
            return Ok();
        }
    }
}
