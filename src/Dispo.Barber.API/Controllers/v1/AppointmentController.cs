using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO.Appointment;
using Dispo.Barber.Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers.v1
{
    [Route("api/v1/appointments")]
    [ApiController]
    public class AppointmentController(IAppointmentAppService appointmentAppService, IinformationChatService informationChatService) : ControllerBase
    {
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken, [FromRoute] long id)
        {
            var result = await appointmentAppService.GetAsync(cancellationToken, id);
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CancellationToken cancellationToken, [FromBody] CreateAppointmentDTO createAppointmentDTO)
        {
            await appointmentAppService.CreateAsync(cancellationToken, createAppointmentDTO);
            return Ok();
        }

        [Authorize]
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


        [Authorize]
        [HttpPatch("{id}/inform-problem")]
        public async Task<IActionResult> InformProblem(CancellationToken cancellationToken, [FromRoute] long id, [FromBody] InformAppointmentProblemDTO informAppointmentProblemDTO)
        {
            await appointmentAppService.InformProblemAsync(cancellationToken, id, informAppointmentProblemDTO);
            return Ok();
        }

        [Authorize]
        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> CancelAppointment(CancellationToken cancellationToken, [FromRoute] long id)
        {
            await appointmentAppService.CancelAppointmentAsync(cancellationToken, id);
            return Ok();
        }

        [HttpPost("generate-suggestions")]
        public async Task<IActionResult> GenerateSuggestions()
        {
            if (await informationChatService.GetSuggestionAppointmentAsync())
                return Ok("Sugestões geradas com sucesso.");
            else return BadRequest();
        }

    }
}
