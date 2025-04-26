using Dispo.Barber.Application.AppServices.Interface;
using Dispo.Barber.Domain.DTOs.Appointment;
using Dispo.Barber.Domain.DTOs.Chat;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Services.Interface;
using Dispo.Barber.Domain.Utils.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Twilio.TwiML.Messaging;

namespace Dispo.Barber.API.Controllers.v1
{
    [AllowAnonymous]
    [Route("api/v1/chat")]
    [ApiController]
    public class ChatController(IInformationChatService informationChatService, 
                                ICustomerService customerService, 
                                ISmsService smsService, 
                                ICacheManager cache, 
                                IAppointmentAppService appointmentAppService) : ControllerBase
    {
        [HttpGet("companies/information/{id}")]
        public async Task<IActionResult> GetCompanyInformationById(CancellationToken cancellationToken, long id)
        {
            try
            {
                var informationChat = await informationChatService.GetInformationChatByIdCompanyAsync(cancellationToken, id);
                return Ok(informationChat);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocorreu um erro ao buscar empressa.", error = ex.Message });
            }
        }

        [HttpGet("customers/phone")]
        public async Task<IActionResult> GetCustomerByPhone([FromQuery] string phone)
        {
            try
            {
                var result = await customerService.GetByPhoneAsync(phone);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocorreu um erro ao buscar o cliente.", error = ex.Message });
            }
        }

        [HttpGet("users/{id}/information")]
        public async Task<IActionResult> GetUserInformationById(CancellationToken cancellationToken, long id)
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

        [HttpGet("users/{idUser}/information-schedules")]
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

        [HttpGet("users/information-get-available-slots")]
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

        [AllowAnonymous]
        [HttpPost("services/information")]
        public async Task<IActionResult> GetServiceInformationById([FromBody] List<long> serviceIds)
        {
            try
            {
                var informationChat = await informationChatService.GetInformationChatByIdService(serviceIds);
                return Ok(informationChat);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocorreu um erro ao buscar as informações de serviço.", error = ex.Message });
            }
        }

        [HttpPost("appointments")]
        public async Task<IActionResult> Create(CancellationToken cancellationToken, [FromBody] CreateAppointmentDTO createAppointmentDTO)
        {
            await appointmentAppService.CreateAsync(cancellationToken, createAppointmentDTO, notifyUsers: true);
            return Ok();
        }

        [HttpPatch("appointments/{id}/cancel")]
        public async Task<IActionResult> CancelAppointment(CancellationToken cancellationToken, [FromRoute] long id)
        {
            await appointmentAppService.CancelAppointmentAsync(cancellationToken, id, notifyUsers: true);
            return Ok();
        }

        [HttpPatch("appointments/{idAppointment}/information-appointment")]
        public async Task<IActionResult> GetInformationAppointmentById(CancellationToken cancellationToken, [FromRoute] long idAppointment)
        {
            try
            {
                var result = await informationChatService.GetInformationAppointmentChatByIdAppointment(cancellationToken, idAppointment);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }
        }

        [HttpPost("appointments/reschedule")]
        public async Task<IActionResult> Reschedule(CancellationToken cancellationToken, [FromBody] CreateAppointmentDTO createAppointmentDTO)
        {
            try
            {
                await appointmentAppService.CancelAppointmentAsync(cancellationToken, createAppointmentDTO.Id, false);
                createAppointmentDTO.Id = 0L;
                await appointmentAppService.CreateAsync(cancellationToken, createAppointmentDTO, reschedule: true, notifyUsers: true);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new { message = $"Ocorreu um erro ao reagendar: {e.Message}" });
            }
        }
    }
}
