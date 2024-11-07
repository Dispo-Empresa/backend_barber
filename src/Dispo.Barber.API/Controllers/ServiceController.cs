using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Service;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers
{
    [Route("api/v1/services")]
    [ApiController]
    public class ServiceController(IServiceAppService serviceAppService, IinformationChatService informationChatService) : ControllerBase
    {
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CancellationToken cancellationToken, [FromBody] CreateServiceDTO createServiceDTO)
        {
            await serviceAppService.CreateAsync(cancellationToken, createServiceDTO);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("information")]
        public async Task<IActionResult> GetInformationChatByService([FromBody] List<long> serviceIds)
        {
            try
            {
                var informationChat = await informationChatService.GetInformationChatByIdService(serviceIds);
                return Ok(informationChat);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocorreu um erro ao buscar as informações de serviço.", error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] long id, CancellationToken cancellationToken)
        {
            var result = await serviceAppService.GetServicesList(id, cancellationToken);
            return Ok(result);
        }
    }
}
