using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Domain.DTO.Company;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers
{
    [Route("api/v1/companies")]
    [ApiController]
    public class CompanyController(ICompanyAppService companyAppService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCompanyDTO companyDTO)
        {
            await companyAppService.CreateAsync(companyDTO);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await companyAppService.GetAllAsync();
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{id}/business-unities")]
        public async Task<IActionResult> Get([FromRoute] long id)
        {
            var result = await companyAppService.GetBusinessUnitiesAsync(id);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] long id, [FromBody] UpdateCompanyDTO updateCompanyDTO)
        {
            await companyAppService.UpdateAsync(id, updateCompanyDTO);
            return Ok();
        }
    }
}
