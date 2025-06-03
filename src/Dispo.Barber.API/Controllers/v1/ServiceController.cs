using Dispo.Barber.Application.AppServices.Interface;
using Dispo.Barber.Domain.DTOs.Service;
using Dispo.Barber.Domain.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers.v1
{
    [Route("api/v1/services")]
    [ApiController]
    public class ServiceController(IServiceAppService serviceAppService, IInformationChatService informationChatService) : ControllerBase
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
        public async Task<IActionResult> GetInformationChatByService(CancellationToken cancellationToken, [FromBody] List<long> serviceIds)
        {
                var informationChat = await informationChatService.GetInformationChatByIdService(cancellationToken, serviceIds);
                return Ok(informationChat);
        }

        //[Authorize] // VALIDAR
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] long id, [FromQuery] bool? activated, CancellationToken cancellationToken)
        {
            var result = await serviceAppService.GetServicesList(cancellationToken, id, activated);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await serviceAppService.GetAllServicesList(cancellationToken);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(CancellationToken cancellationToken, [FromRoute] long id, [FromBody] UpdateServiceDTO updateServiceDTO)
        {
            await serviceAppService.UpdateAsync(cancellationToken, id, updateServiceDTO);
            return Ok();
        }

        [Authorize]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeStatus(CancellationToken cancellationToken, [FromRoute] long id, [FromBody] ChangeServiceStatusDTO changeServiceStatusDTO)
        {
            await serviceAppService.ChangeStatusAsync(cancellationToken, id, changeServiceStatusDTO.Status);
            return Ok();
        }
    }
}
