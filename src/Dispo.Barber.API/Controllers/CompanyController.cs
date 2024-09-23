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
    }
}
