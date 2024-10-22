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
        public async Task<IActionResult> Create(CancellationToken cancellationToken, [FromBody] CreateUserDTO createUserDTO)
        {
            await userAppService.CreateAsync(cancellationToken, createUserDTO);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(CancellationToken cancellationToken, [FromRoute] long id, [FromBody] UpdateUserDTO updateUserDTO)
        {
            await userAppService.UpdateAsync(cancellationToken, id, updateUserDTO);
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
        public async Task<IActionResult> GetUserAppointments(CancellationToken cancellationToken, [FromRoute] long id, [FromQuery] GetUserAppointmentsDTO getUserAppointmentsDTO)
        {
            var result = await userAppService.GetUserAppointmentsAsync(cancellationToken, id, getUserAppointmentsDTO);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{id}/schedules")]
        public async Task<IActionResult> GetUserSchedules(CancellationToken cancellationToken, [FromRoute] long id)
        {
            var result = await userAppService.GetUserSchedulesAsync(cancellationToken, id);
            return Ok(result);
        }


        [AllowAnonymous]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeStatus(CancellationToken cancellationToken, [FromRoute] long id, [FromBody] ChangeStatusDTO changeStatusDTO)
        {
            await userAppService.ChangeStatusAsync(cancellationToken, id, changeStatusDTO);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPatch("{id}/password")]
        public async Task<IActionResult> ChangePassword(CancellationToken cancellationToken, [FromRoute] long id, [FromBody] ChangePasswordDTO changePasswordDTO)
        {
            await userAppService.ChangePasswordAsync(cancellationToken, id, changePasswordDTO);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("{id}/information")]
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

        [AllowAnonymous]
        [HttpGet("id-by-phone")]
        public async Task<IActionResult> GetUserIdByPhone(CancellationToken cancellationToken, [FromQuery] string phone)
        {
            var result = await userAppService.GetUserIdByPhone(cancellationToken, phone);
            return Ok(result);
        }
    }
}
