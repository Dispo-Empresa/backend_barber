using Dispo.Barber.Application.AppServices.Interfaces;
using Dispo.Barber.Domain.DTOs.Company;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers.v1
{
    [Authorize]
    [Route("api/v1/licences-management")]
    [ApiController]
    public class LicenceManagementController(ILicenceManagementAppService licenceManagementAppService) : ControllerBase
    {
        [HttpPatch("companies/{companyId}/change-license-plan")]
        public async Task<IActionResult> ChangeLicensePlan([FromRoute] long companyId, [FromBody] ChangeLicensePlanDTO changeLicensePlanDTO, CancellationToken cancellationToken)
        {
            await licenceManagementAppService.ChangeLicensePlan(companyId, changeLicensePlanDTO, cancellationToken);
            return Ok();
        }
    }
}
