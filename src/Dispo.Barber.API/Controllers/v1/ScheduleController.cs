using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Domain.DTO.Schedule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers.v1
{
    [Route("api/v1/schedules")]
    [ApiController]
    public class ScheduleController(IScheduleAppService scheduleAppService) : ControllerBase
    {
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CancellationToken cancellationToken, [FromBody] CreateScheduleDTO createScheduleDTO)
        {
            await scheduleAppService.CreateAsync(cancellationToken, createScheduleDTO);
            return Ok();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, [FromRoute] long id)
        {
            await scheduleAppService.DeleteAsync(cancellationToken, id);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(CancellationToken cancellationToken, [FromRoute] long id, [FromBody] UpdateScheduleDTO updateScheduleDTO)
        {
            await scheduleAppService.UpdateAsync(cancellationToken, id, updateScheduleDTO);
            return Ok();
        }
    }
}
