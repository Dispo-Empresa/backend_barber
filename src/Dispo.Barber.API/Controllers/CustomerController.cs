using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Application.AppService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers
{
    [Route("api/v1/customers")]
    [ApiController]
    public class CustomerController(ICustomerAppService customerAppService, ICustomerService customerService) : ControllerBase
    {

        [AllowAnonymous]
        [HttpGet("{phone}")]
        public async Task<IActionResult> GetByPhone(string phone)
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

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken, [FromQuery] string search)
        {
            var result = await customerAppService.GetForAppointment(cancellationToken, search);
            return Ok(result);
        }
    }
}
