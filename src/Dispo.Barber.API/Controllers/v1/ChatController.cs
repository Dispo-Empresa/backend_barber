using Dispo.Barber.Application.AppServices.Interface;
using Dispo.Barber.Domain.DTOs.Appointment;
using Dispo.Barber.Domain.DTOs.Chat;
using Dispo.Barber.Domain.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers.v1
{
    [AllowAnonymous]
    [Route("api/v1/chat")]
    [ApiController]
    public class ChatController(IInformationChatService informationChatService, 
                                ICustomerService customerService, 
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
                if (result == null)
                    return StatusCode(500, new { message = "Ocorreu um erro ao buscar o cliente."});

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
                if (result == null)
                    return StatusCode(500, new { message = "Ocorreu um erro ao buscar os horários de agedamento." });

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
                if (result == null)
                    return StatusCode(500, new { message = "Ocorreu um erro ao buscar os horários disponíveis." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching available slots.", Details = ex.Message });
            }
        }

        [HttpPost("services/information")]
        public async Task<IActionResult> GetServiceInformationById([FromBody] List<long> serviceIds)
        {
            try
            {
                var informationChat = await informationChatService.GetInformationChatByIdService(serviceIds);
                if (informationChat == null)
                    return StatusCode(500, new { message = "Ocorreu um erro ao buscar as informações de serviço." });

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
            try
            {
                await appointmentAppService.CreateAsync(cancellationToken, createAppointmentDTO, notifyUsers: true);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ocorreu um erro ao criar o agendamento. {ex.Message}", error = ex.Message });
            }
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
            var result = await informationChatService.GetInformationAppointmentChatByIdAppointment(cancellationToken, idAppointment);
            return Ok(result);
        }

        [HttpPost("appointments/reschedule")]
        public async Task<IActionResult> Reschedule(CancellationToken cancellationToken, [FromBody] CreateAppointmentDTO createAppointmentDTO)
        {
            await appointmentAppService.RescheduleAsync(cancellationToken, createAppointmentDTO);
            return Ok();
        }
    }
}
