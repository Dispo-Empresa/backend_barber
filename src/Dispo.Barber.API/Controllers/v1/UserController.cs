using Dispo.Barber.Application.AppServices.Interface;
using Dispo.Barber.Domain.DTOs.Chat;
using Dispo.Barber.Domain.DTOs.User;
using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers.v1
{
    [Route("api/v1/users")]
    [ApiController]
    public class UserController(IUserAppService userAppService,
                                IInformationChatService informationChatService,
                                IDashboardAppService dashboardAppService) : ControllerBase
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

        [Authorize]
        [HttpGet("{id}/appointments/next")]
        public async Task<IActionResult> GetUserAppointments(CancellationToken cancellationToken, [FromRoute] long id)
        {
            return Ok(await userAppService.GetNextAppointmentsAsync(cancellationToken, id));
        }

        [HttpGet("{id}/schedules")]
        public async Task<IActionResult> GetUserSchedules(CancellationToken cancellationToken, [FromRoute] long id)
        {
            var result = await userAppService.GetUserSchedulesAsync(cancellationToken, id);
            return Ok(result);
        }

        [HttpGet("{id}/breaks/{dayOfWeek}")]
        public async Task<IActionResult> GetUserBreaks(CancellationToken cancellationToken, [FromRoute] long id, DayOfWeek dayOfWeek)
        {
            var result = await userAppService.GetUserBreaksAsync(cancellationToken, id, dayOfWeek);
            return Ok(result);
        }

        [HttpGet("{id}/days-off")]
        public async Task<IActionResult> GetUserDaysOff(CancellationToken cancellationToken, [FromRoute] long id)
        {
            var result = await userAppService.GetUserDaysOffAsync(cancellationToken, id);
            return Ok(result);
        }

        [Authorize]
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

        //[Authorize]
        [HttpGet("info-by-phone")]
        public async Task<IActionResult> GetUserInfoPendingByPhone(CancellationToken cancellationToken, [FromQuery] string phone, [FromQuery] UserStatus status = UserStatus.Active)
        {
            var result = await userAppService.GetUserInfoByPhone(cancellationToken, phone, status);
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
            return Ok(await informationChatService.GetAvailableSlotsAsync(cancellationToken, requestDto));
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

        [Authorize]
        [HttpGet("{id}/customers")]
        public async Task<IActionResult> GetUserCustomers(CancellationToken cancellationToken, [FromRoute] long id)
        {
            return Ok(await userAppService.GetUserCustomersAsync(cancellationToken, id));
        }

        [Authorize]
        [HttpGet("{id}/enabled-services")]
        public async Task<IActionResult> GetEnabledServices(CancellationToken cancellationToken, [FromRoute] long id)
        {
            return Ok(await userAppService.GetEnabledServicesAsync(cancellationToken, id));
        }

        [Authorize]
        [HttpGet("{id}/services")]
        public async Task<IActionResult> GetServices(CancellationToken cancellationToken, [FromRoute] long id)
        {
            return Ok(await userAppService.GetServicesAsync(cancellationToken, id));
        }

        [Authorize]
        [HttpPost("{id}/photo")]
        public async Task<IActionResult> UploadPhoto(CancellationToken cancellationToken, [FromRoute] long id, IFormFile? file)
        {
            byte[]? byteArrayImage = null;

            if (file != null && file.Length != 0)
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream, cancellationToken);
                byteArrayImage = stream.ToArray();
            }

            await userAppService.UploadImageAsync(cancellationToken, id, byteArrayImage);
            return Ok();
        }

        [Authorize]
        [HttpPost("{id}/appointments/cancel-by-date")]
        public async Task<IActionResult> CancelAllByDate(CancellationToken cancellationToken, [FromRoute] long id, [FromBody] DateTime date)
        {
            await userAppService.CancelAllByDateAsync(cancellationToken, id, date);
            return Ok();
        }

        [Authorize]
        [HttpPatch("{id}/services/{serviceId}/stop-providing")]
        public async Task<IActionResult> StopProvidingService(CancellationToken cancellationToken, [FromRoute] long id, long serviceId)
        {
            await userAppService.StopProvidingServiceAsync(cancellationToken, id, serviceId);
            return Ok();
        }

        [Authorize]
        [HttpPatch("{id}/services/{serviceId}/start-providing")]
        public async Task<IActionResult> StartProvidingServiceAsync(CancellationToken cancellationToken, [FromRoute] long id, long serviceId)
        {
            await userAppService.StartProvidingServiceAsync(cancellationToken, id, serviceId);
            return Ok();
        }

        [HttpPatch("{id}/device-token/{deviceToken}")]
        public async Task<IActionResult> ChangeDeviceToken(CancellationToken cancellationToken, [FromRoute] long id, string deviceToken)
        {
            await userAppService.ChangeDeviceToken(cancellationToken, id, deviceToken);
            return Ok();
        }

        [Authorize]
        [HttpPost("{id}/appointments/cancel-scheduled")]
        public async Task<IActionResult> CancelAllScheduledAsync(CancellationToken cancellationToken, [FromRoute] long id)
        {
            await userAppService.CancelAllScheduledAsync(cancellationToken, id);
            return Ok();
        }

        [Authorize]
        [HttpPost("{id}/appointments/cancel-scheduled-by-range-date")]
        public async Task<IActionResult> CancelAllUserScheduledByDateAsync(CancellationToken cancellationToken, [FromRoute] long id, [FromBody] GetUserAppointmentsDTO request)
        {
            await userAppService.CancelAllUserScheduledByDateAsync(cancellationToken, id, request.StartDate ?? DateTime.Now, request.EndDate ?? DateTime.Now);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("{id}/company")]
        public async Task<IActionResult> GetCompanyIdByIdAsync(CancellationToken cancellationToken, [FromRoute] long id)
        {
            return Ok(await userAppService.GetCompanyIdByIdAsync(cancellationToken, id));
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(CancellationToken cancellationToken, [FromRoute] long id)
        {
            await userAppService.RemoveAsync(cancellationToken, id);
            return Ok();
        }

        [Authorize]
        [HttpGet("{id}/unread-notifications")]
        public async Task<IActionResult> GetUnreadNotificationsCount(CancellationToken cancellationToken, [FromRoute] long id)
        {
            var response = await userAppService.GetUnreadNotificationsCountAsync(cancellationToken, id);
            return Ok(response);
        }

        [Authorize]
        [HttpPatch("{id}/reset-unread-notifications")]
        public async Task<IActionResult> ResetUnreadNotifications(CancellationToken cancellationToken, [FromRoute] long id)
        {
            await userAppService.ResetUnreadNotificationsAsync(cancellationToken, id);
            return Ok();
        }
    }
}