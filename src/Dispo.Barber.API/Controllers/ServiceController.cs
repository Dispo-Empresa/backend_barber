using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Domain.DTO.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers
{
    [Route("api/v1/services")]
    [ApiController]
    public class ServiceController(IServiceAppService serviceAppService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateServiceDTO createServiceDTO)
        {
            await serviceAppService.CreateAsync(createServiceDTO);
            return Ok();
        }
    }
}
