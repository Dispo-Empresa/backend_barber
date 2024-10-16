using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Domain.DTO.Appointment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers
{
    [Route("api/v1/appointments")]
    [ApiController]
    public class AppointmentController(IAppointmentAppService appointmentAppService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken, [FromRoute] long id)
        {
            var result = await appointmentAppService.GetAsync(cancellationToken, cancellationToken, id);
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
        [HttpPatch("{id}/inform-problem")]
        public async Task<IActionResult> InformProblem(CancellationToken cancellationToken, [FromRoute] long id, [FromBody] InformAppointmentProblemDTO informAppointmentProblemDTO)
        {
            await appointmentAppService.InformProblemAsync(cancellationToken, id, informAppointmentProblemDTO);
            return Ok();
        }
    }
}
