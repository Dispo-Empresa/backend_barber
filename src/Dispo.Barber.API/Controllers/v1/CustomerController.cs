using Dispo.Barber.Application.AppServices.Interface;
using Dispo.Barber.Domain.DTOs.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers.v1
{
    [Authorize]
    [Route("api/v1/customers")]
    [ApiController]
    public class CustomerController(ICustomerAppService customerAppService) : ControllerBase
    {

        [HttpGet("search/{search}")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken, [FromRoute] string search)
        {
            var result = await customerAppService.GetForAppointment(cancellationToken, search);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(CancellationToken cancellationToken, [FromRoute] long id)
        {
            var result = await customerAppService.GetByIdAsync(cancellationToken, id);
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

        [HttpPost]
        public async Task<IActionResult> Create(CancellationToken cancellationToken, [FromBody] CustomerDTO customerDTO)
        {
            await customerAppService.CreateAsync(cancellationToken, customerDTO);
            return Ok();
        }
    }
}
