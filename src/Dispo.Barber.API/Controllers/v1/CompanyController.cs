using Dispo.Barber.Application.AppServices.Interfaces;
using Dispo.Barber.Domain.DTOs.Company;
using Dispo.Barber.Domain.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers.v1
{
    [Route("api/v1/companies")]
    [ApiController]
    public class CompanyController(ICompanyAppService companyAppService, IInformationChatService informationChatService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create(CancellationToken cancellationToken, [FromBody] CreateCompanyDTO companyDTO)
        {
            await companyAppService.CreateAsync(cancellationToken, companyDTO);
            return Ok();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var result = await companyAppService.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id}/business-unities")]
        public async Task<IActionResult> GetBusinessUnities(CancellationToken cancellationToken, [FromRoute] long id)
        {
            var result = await companyAppService.GetBusinessUnitiesAsync(cancellationToken, id);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(CancellationToken cancellationToken, [FromRoute] long id, [FromBody] UpdateCompanyDTO updateCompanyDTO)
        {
            await companyAppService.UpdateAsync(cancellationToken, id, updateCompanyDTO);
            return Ok();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken, [FromRoute] long id)
        {
            var result = await companyAppService.GetAsync(cancellationToken, id);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("information/{id}")]
        public async Task<IActionResult> GetInformationChatById(CancellationToken cancellationToken, long id)
        {
            try
            {
                var informationChat = await informationChatService.GetInformationChatByIdCompanyAsync(cancellationToken, id);
                return Ok(informationChat);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocorreu um erro ao buscar empressa.", error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{id}/users")]
        public async Task<IActionResult> GetUsers(CancellationToken cancellationToken, long id)
        {
            return Ok(await companyAppService.GetUsersAsync(cancellationToken, id));
        }
    }
}
