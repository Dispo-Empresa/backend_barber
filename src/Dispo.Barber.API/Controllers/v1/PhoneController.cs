using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers.v1
{
    [Route("api/v1/phone")]
    [ApiController]
    public class PhoneController(ISmsService smsService) : ControllerBase
    {
        [HttpPost("{phone}")]
        public async Task<IActionResult> PostPhoneNumber(string phone)
        {
            try
            {
                var verificationCode = await smsService.SendMessageAsync(phone, "Seu código de verificação é: {code}", MessageType.Sms);
                return Ok(new { message = "Código enviado com sucesso.", code = verificationCode });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao enviar SMS: {ex.Message}" });
            }
        }
    }
}
