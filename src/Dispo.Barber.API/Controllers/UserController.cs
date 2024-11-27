using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO.Chat;
using Dispo.Barber.Domain.DTO.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    public class UserController(IUserAppService userAppService, IinformationChatService informationChatService, IDashboardAppService dashboardAppService) : ControllerBase
    {
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CancellationToken cancellationToken, [FromBody] CreateUserDTO createUserDTO)
        {
            await userAppService.CreateAsync(cancellationToken, createUserDTO);
            return Ok();
        }

        //[Authorize] // VALIDAR
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(CancellationToken cancellationToken, [FromRoute] long id, [FromBody] UpdateUserDTO updateUserDTO)
        {
            await userAppService.UpdateAsync(cancellationToken, id, updateUserDTO);
            return Ok();
        }

        [Authorize]
        [HttpPost("{id}/services")]
        public async Task<IActionResult> AddServiceToUser(CancellationToken cancellationToken, [FromRoute] long id, [FromBody] AddServiceToUserDTO addServiceToUserDTO)
        {
            await userAppService.AddServiceToUserAsync(cancellationToken, id, addServiceToUserDTO.Services);
            return Ok();
        }

        [Authorize]
        [HttpGet("{id}/appointments")]
        public async Task<IActionResult> GetUserAppointments(CancellationToken cancellationToken, [FromRoute] long id, [FromQuery] GetUserAppointmentsDTO getUserAppointmentsDTO)
        {
            var result = await userAppService.GetUserAppointmentsAsync(cancellationToken, id, getUserAppointmentsDTO);
            return Ok(result);
        }

        //[Authorize] // VALIDAR
        [HttpGet("{id}/schedules")]
        public async Task<IActionResult> GetUserSchedules(CancellationToken cancellationToken, [FromRoute] long id)
        {
            var result = await userAppService.GetUserSchedulesAsync(cancellationToken, id);
            return Ok(result);
        }


        [Authorize]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeStatus(CancellationToken cancellationToken, [FromRoute] long id, [FromBody] ChangeStatusDTO changeStatusDTO)
        {
            await userAppService.ChangeStatusAsync(cancellationToken, id, changeStatusDTO);
            return Ok();
        }

        [Authorize]
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

        [Authorize]
        [HttpGet("id-by-phone")]
        public async Task<IActionResult> GetUserIdByPhone(CancellationToken cancellationToken, [FromQuery] string phone)
        {
            var result = await userAppService.GetUserIdByPhone(cancellationToken, phone);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{idUser}/information-schedules")]
        public async Task<IActionResult> GetUserSchedulesInformationByUserId(CancellationToken cancellationToken, [FromRoute] long idUser)
        {
            try
            {
                var result = await informationChatService.GetUserAppointmentsByUserIdAsync(cancellationToken, idUser);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("{id}/dashboards")]
        public async Task<IActionResult> BuildDashboard(CancellationToken cancellationToken, [FromRoute] long id)
        {
            return Ok(await dashboardAppService.BuildDashboardForUser(cancellationToken, id));
        }

        [AllowAnonymous]
        [HttpGet("information-get-available-slots")]
        public async Task<IActionResult> GetAvailableSlotsAsync(CancellationToken cancellationToken, [FromQuery] AvailableSlotRequestDto requestDto)
        {
            try
            {
                var result = await informationChatService.GetAvailableSlotsAsync(cancellationToken, requestDto);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching available slots.", Details = ex.Message });
            }
        }

        [Tags("Link")]
        [Authorize]
        [HttpGet("{companySlug}/{userSlug}")]
        public async Task<IActionResult> GetByCompanyAndUserSlug(CancellationToken cancellationToken, [FromRoute] string companySlug, [FromRoute] string userSlug)
        {
            return Ok(await userAppService.GetByCompanyAndUserSlugAsync(cancellationToken, companySlug, userSlug));
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(CancellationToken cancellationToken, [FromRoute] long id)
        {
            return Ok(await userAppService.GetByIdAsync(cancellationToken, id));
        }
    }
}
