using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO.Company;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers
{
    [Route("api/v1/companies")]
    [ApiController]
    public class CompanyController(ICompanyAppService companyAppService, IinformationChatService informationChatService) : ControllerBase
    {
        [Authorize]
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
    }
}
