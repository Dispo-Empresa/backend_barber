using Dispo.Barber.Application.AppService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers
{
    [Route("api/v1/business-unities")]
    [ApiController]
    public class BusinessUnityController(IBusinessUnityAppService businessUnityAppService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("{id}/users")]
        public async Task<IActionResult> GetUsers([FromRoute] long id)
        {
            var result = await businessUnityAppService.GetUsersAsync(id);
            return Ok(result);
        }
    }
}
