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
        [HttpGet("phone")]
        public async Task<IActionResult> GetByPhone([FromQuery] string phone)
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

        [HttpGet("search")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken, [FromRoute] string search)
        {
            var result = await customerAppService.GetForAppointment(cancellationToken, search);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(CancellationToken cancellationToken, [FromRoute] long id)
        {
            var result = await customerAppService.GetCustomerAppointmentsAsync(cancellationToken, id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await customerAppService.GetCustomersAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}/appointments")]
        public async Task<IActionResult> GetAppointments(CancellationToken cancellationToken, [FromRoute] long id)
        {
            var result = await customerAppService.GetCustomerAppointmentsAsync(cancellationToken, id);
            return Ok(result);
        }
    }
}
