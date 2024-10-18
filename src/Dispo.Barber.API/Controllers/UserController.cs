using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace Dispo.Barber.API.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    public class UserController(IUserAppService userAppService, IinformationChatService informationChatService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDTO createUserDTO)
        {
            await userAppService.CreateAsync(createUserDTO);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] long id, [FromBody] UpdateUserDTO updateUserDTO)
        {
            await userAppService.UpdateAsync(id, updateUserDTO);
            return Ok();
        }


        [AllowAnonymous]
        [HttpPost("{id}/services")]
        public async Task<IActionResult> AddServiceToUser(CancellationToken cancellationToken, [FromRoute] long id, [FromBody] AddServiceToUserDTO addServiceToUserDTO)
        {
            await userAppService.AddServiceToUserAsync(cancellationToken, id, addServiceToUserDTO);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("{id}/appointments")]
        public async Task<IActionResult> GetUserAppointments([FromRoute] long id, [FromQuery] GetUserAppointmentsDTO getUserAppointmentsDTO)
        {
            var result = await userAppService.GetUserAppointmentsAsync(id, getUserAppointmentsDTO);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{id}/schedules")]
        public async Task<IActionResult> GetUserSchedules([FromRoute] long id)
        {
            var result = await userAppService.GetUserSchedulesAsync(id);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPatch("{id}/disable")]
        public async Task<IActionResult> Disable([FromRoute] long id)
        {
            await userAppService.DisableUserAsync(id);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("information/{id}")]
        public async Task<IActionResult> GetInformationChatById(CancellationToken cancellationToken, long id)
        {
            try
            {
                var informationChat = await informationChatService.GetInformationChatByIdUser(cancellationToken, id);
                return Ok(informationChat);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocorreu um erro ao buscar empressa.", error = ex.Message });
            }
        }
    }
}
