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
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentDTO createAppointmentDTO)
        {
            await appointmentAppService.CreateAsync(createAppointmentDTO);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPatch("{id}/inform-problem")]
        public async Task<IActionResult> InformProblem([FromRoute] long id, [FromBody] InformAppointmentProblemDTO informAppointmentProblemDTO)
        {
            await appointmentAppService.InformProblemAsync(id, informAppointmentProblemDTO);
            return Ok();
        }
    }
}
