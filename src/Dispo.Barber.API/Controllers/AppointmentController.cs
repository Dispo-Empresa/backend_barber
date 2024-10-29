using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO.Appointment;
using Dispo.Barber.Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace Dispo.Barber.API.Controllers
{
    [Route("api/v1/appointments")]
    [ApiController]
    public class AppointmentController(IAppointmentAppService appointmentAppService, IinformationChatService informationChatService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken, [FromRoute] long id)
        {
            var result = await appointmentAppService.GetAsync(cancellationToken, id);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create(CancellationToken cancellationToken, [FromBody] CreateAppointmentDTO createAppointmentDTO)
        {
            await appointmentAppService.CreateAsync(cancellationToken, createAppointmentDTO);
            return Ok();
        }

        [AllowAnonymous]
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


        [AllowAnonymous]
        [HttpPatch("{id}/inform-problem")]
        public async Task<IActionResult> InformProblem(CancellationToken cancellationToken, [FromRoute] long id, [FromBody] InformAppointmentProblemDTO informAppointmentProblemDTO)
        {
            await appointmentAppService.InformProblemAsync(cancellationToken, id, informAppointmentProblemDTO);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> CancelAppointment(CancellationToken cancellationToken, [FromRoute] long id)
        {
            await appointmentAppService.CancelAppointmentAsync(cancellationToken, id);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("{idUser}/infomation")]
        public async Task<IActionResult> GetInformationAvailableDateTimesByUser(CancellationToken cancellationToken, [FromRoute] long idUser)
        {
            var InformationAppointmentChatDto = await informationChatService.GetAvailableDateTimessByUserIdAsync(cancellationToken, idUser);
            return Ok(InformationAppointmentChatDto);
        }
    }
}
