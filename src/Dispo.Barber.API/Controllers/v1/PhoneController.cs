using Dispo.Barber.Application.Service.Interface;
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

                var codeRandom = new Random().Next(1000, 9999).ToString();

                var fullMessageBody = $"Seu código de verificação é: {codeRandom}";

                smsService.SendMessageAsync(phone, fullMessageBody);

                return Ok(new { message = "Código enviado com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao enviar SMS: {ex.Message}" });
            }
        }
    }
}
