using Dispo.Barber.Application.AppServices.Interface;
using Dispo.Barber.Domain.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers.v2
{
    [Route("api/v2/users")]
    [ApiController]
    public class UserController(IUserAppService userAppService) : ControllerBase
    {
        [Authorize]
        [HttpGet("{id}/appointments")]
        public async Task<IActionResult> GetUserAppointments(CancellationToken cancellationToken, [FromRoute] long id, [FromQuery] GetUserAppointmentsDTO getUserAppointmentsDTO)
        {
            var result = await userAppService.GetAppointmentsAsyncV2(cancellationToken, id, getUserAppointmentsDTO);
            return Ok(result);
        }
    }
}