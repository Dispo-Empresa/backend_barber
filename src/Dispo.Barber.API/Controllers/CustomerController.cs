using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers
{
    [Route("api/v1/customer")]
    [ApiController]
    public class CustomerController(ICustomerService customerService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerDTO customerDTO)
        {
            try
            {
                var result = await customerService.CreateAsync(customerDTO); 
                return Ok(result); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocorreu um erro ao criar o cliente.", error = ex.Message });
            }
        }

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
    }
}
