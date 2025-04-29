using Dispo.Barber.Domain.DTOs.Phone;
using Dispo.Barber.Domain.Services.Interface;
using Dispo.Barber.Domain.Utils.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers.v1
{
    [Route("api/v1/phone")]
    [ApiController]
    public class PhoneController(ISmsService smsService, ICacheManager cache) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> GenerateSmsCode([FromBody] GenerateSmsCodePhone request)
        {
            try
            {
                var codeRandom = new Random().Next(1000, 9999).ToString();

                var fullMessageBody = $"Seu código de verificação do Aura é {codeRandom}. Use-o para continuar.";

                await smsService.SendMessageAsync(request.Phone, fullMessageBody);

                cache.Add(request.Phone, codeRandom);

                return Ok(new { message = "Enviamos o seu código de verificação. Dá uma olhadinha no seu SMS!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Não conseguimos enviar o SMS agora. Por favor, tente novamente em instantes." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSmsCode([FromQuery] string phone, [FromQuery] string sms)
        {
            try
            {
                var smsInCache = cache.Get(phone);

                // Excluir o token do cache caso já está válido

                if (smsInCache == null)
                {
                    throw new Exception("O código expirou.");
                }

                return Ok(new { data = smsInCache == sms });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao buscar o código do sms: {ex.Message}" });
            }
        }
    }
}
