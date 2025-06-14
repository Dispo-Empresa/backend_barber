﻿using Dispo.Barber.Application.AppServices.Interfaces;
using Dispo.Barber.Domain.DTOs.Appointment;
using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers.v1
{
    [Route("api/v1/appointments")]
    [Authorize]
    [ApiController]
    public class AppointmentController(IAppointmentAppService appointmentAppService, 
                                       IInformationChatService informationChatService,
                                       IInformationSuggestionAppService informationSuggestionAppService ) : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken, [FromRoute] long id)
        {
            var result = await appointmentAppService.GetAsync(cancellationToken, id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CancellationToken cancellationToken, [FromBody] CreateAppointmentDTO createAppointmentDTO)
        {
            await appointmentAppService.CreateAsync(cancellationToken, createAppointmentDTO);
            return Ok();
        }

        [HttpPost("create-by-services")]
        public async Task<IActionResult> CreateByServices(CancellationToken cancellationToken, [FromBody] CreateAppointmentServicosDTO createCreateAppointmentServicosDTO)
        {
            var createAppointmentDTO = new CreateAppointmentDTO
            {
                Date = createCreateAppointmentServicosDTO.Date,
                CustomerObservation = createCreateAppointmentServicosDTO.CustomerObservation,
                AcceptedUserObservation = createCreateAppointmentServicosDTO.AcceptedUserObservation,
                AcceptedUserId = createCreateAppointmentServicosDTO.AcceptedUserId,
                BusinessUnityId = createCreateAppointmentServicosDTO.BusinessUnityId,
                Services = createCreateAppointmentServicosDTO.ServiceIds,
                Status = AppointmentStatus.Scheduled,
                Customer = createCreateAppointmentServicosDTO.Customer
            };

            await appointmentAppService.CreateAsync(cancellationToken, createAppointmentDTO);
            return Ok();
        }


        [HttpPatch("{id}/inform-problem")]
        public async Task<IActionResult> InformProblem(CancellationToken cancellationToken, [FromRoute] long id, [FromBody] InformAppointmentProblemDTO informAppointmentProblemDTO)
        {
            await appointmentAppService.InformProblemAsync(cancellationToken, id, informAppointmentProblemDTO);
            return Ok();
        }

        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> CancelAppointment(CancellationToken cancellationToken, [FromRoute] long id)
        {
            await appointmentAppService.CancelAppointmentAsync(cancellationToken, id);
            return Ok();
        }

        [HttpPatch("cancel")]
        public async Task<IActionResult> CancelAppointments(CancellationToken cancellationToken, [FromBody] List<long> appointmentIds)
        {
            await appointmentAppService.CancelAppointmentsAsync(cancellationToken, appointmentIds);
            return Ok();
        }

        [HttpPost("generate-suggestions")]
        public async Task<IActionResult> GenerateSuggestions()
        {
            if (await informationSuggestionAppService.GetSuggestionAppointmentAsync())
                return Ok("Sugestões geradas com sucesso.");
            else return BadRequest();
        }

        [HttpGet("schedules/conflicts")]
        public async Task<IActionResult> GetScheduleConflictsAsync(CancellationToken cancellationToken, [FromQuery] long userId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var appointments = await appointmentAppService.GetScheduleConflictsAsync(cancellationToken, userId, startDate, endDate);
            return Ok(appointments);
        }

        [HttpGet("schedules/conflicts-by-time")]
        public async Task<IActionResult> GetScheduleConflictsAsync(CancellationToken cancellationToken, [FromQuery] long userId, [FromQuery] string startTime, [FromQuery] string endTime, [FromQuery] DayOfWeek dayOfWeek, [FromQuery] bool isBreak = false)
        {
            var start = TimeSpan.Parse(startTime);
            var end = TimeSpan.Parse(endTime);

            var appointments = await appointmentAppService.GetScheduleConflictsAsync(cancellationToken, userId, start, end, dayOfWeek, isBreak);
            return Ok(appointments);
        }

        [HttpPatch("{idAppointment}/information-appointment")]
        public async Task<IActionResult> GetInformationByAppointmentId(CancellationToken cancellationToken, [FromRoute] long idAppointment)
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
    }
}
