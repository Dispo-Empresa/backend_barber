using Dispo.Barber.Application.AppServices.Interfaces;
using Dispo.Barber.Domain.DTOs.Phone.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers.v1
{
    [Route("api/v1/phone")]
    [ApiController]
    public class PhoneController(ITokenConfirmationAppService tokenConfirmationAppService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> GenerateSmsCode([FromBody] GenerateSmsCodePhoneRequest request)
        {
            await tokenConfirmationAppService.GenerateCodeConfirmation(request.Phone);
            return Ok();          
        }

        [HttpGet]
        public async Task<IActionResult> GetSmsCode([FromQuery] string phone, [FromQuery] string sms)
        {
            var response = await tokenConfirmationAppService.ValidateCodeConfirmation(phone, sms);
            return Ok(response);     
        }
    }
}
