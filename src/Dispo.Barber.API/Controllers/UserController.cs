using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Domain.DTO.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    public class UserController(IUserAppService userAppService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("{id}/services")]
        public async Task<IActionResult> Create([FromRoute] long id, [FromBody] AddServiceToUserDTO addServiceToUserDTO)
        {
            await userAppService.AddServiceToUserAsync(id, addServiceToUserDTO);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("{id}/appointments")]
        public async Task<IActionResult> GetUserAppointments([FromRoute] long id, [FromQuery] GetUserAppointmentsDTO getUserAppointmentsDTO)
        {
            var result = await userAppService.GetUserAppointmentsAsync(id, getUserAppointmentsDTO);
            return Ok(result);
        }
    }
}
