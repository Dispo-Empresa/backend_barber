using Dispo.Barber.Application.AppService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers
{
    [Route("api/v1/customers")]
    [ApiController]
    public class CustomerController(ICustomerAppService customerAppService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken, [FromQuery] string search)
        {
            var result = await customerAppService.GetForAppointment(cancellationToken, search);
            return Ok(result);
        }
    }
}
