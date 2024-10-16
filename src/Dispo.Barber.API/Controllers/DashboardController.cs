using Dispo.Barber.Application.AppService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers
{
    [Route("api/v1/dashboards")]
    [ApiController]
    public class DashboardController(IDashboardAppService dashboardAppService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> BuildDashboard(CancellationToken cancellationToken, [FromRoute] long id)
        {
            return Ok(await dashboardAppService.BuildDashboardForUser(cancellationToken, id));
        }
    }
}
