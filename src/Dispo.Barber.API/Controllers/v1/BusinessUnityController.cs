using Dispo.Barber.Application.AppService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers.v1
{
    [Route("api/v1/business-unities")]
    [ApiController]
    public class BusinessUnityController(IBusinessUnityAppService businessUnityAppService) : ControllerBase
    {
        [Authorize]
        [HttpGet("{id}/users")]
        public async Task<IActionResult> GetUsers(CancellationToken cancellationToken, [FromRoute] long id)
        {
            var result = await businessUnityAppService.GetUsersAsync(cancellationToken, id);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id}/users/pending")]
        public async Task<IActionResult> GetPendingUsers(CancellationToken cancellationToken, [FromRoute] long id)
        {
            var result = await businessUnityAppService.GetPendingUsersAsync(cancellationToken, id);
            return Ok(result);
        }

        [HttpGet("{id}/customers")]
        public async Task<IActionResult> GetCustomers(CancellationToken cancellationToken, [FromRoute] long id)
        {
            var result = await businessUnityAppService.GetCustomersAsync(cancellationToken, id);
            return Ok(result);
        }
    }
}
