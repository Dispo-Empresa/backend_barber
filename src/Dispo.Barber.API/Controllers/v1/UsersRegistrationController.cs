using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Domain.DTO.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers.v1
{
    [Authorize]
    [Route("api/v1/user-registrations")]
    [ApiController]
    public class UsersRegistrationController(IUserAppService userAppService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("create-owner-user")]
        public async Task<IActionResult> CreateOwnerUserAsync([FromBody] CreateOwnerUserDTO createOwnerUserDto, CancellationToken cancellationToken)
        {
            await userAppService.CreateOwnerUserAsync(cancellationToken, createOwnerUserDto);
            return Ok();
        }

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
