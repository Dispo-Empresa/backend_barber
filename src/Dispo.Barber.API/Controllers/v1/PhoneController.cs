using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.Utils.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers.v1
{
    [Route("api/v1/phone")]
    [ApiController]
    public class PhoneController(ISmsService smsService, ICacheManager cache) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> GenerateSmsCode([FromBody] string phone)
        {
            try
            {
                var codeRandom = new Random().Next(1000, 9999).ToString();

                var fullMessageBody = $"Seu código de verificação é: {codeRandom}";

                await smsService.SendMessageAsync(phone, fullMessageBody);

                cache.Add(phone, codeRandom);

                return Ok(new { message = "Código enviado com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao enviar SMS: {ex.Message}" });
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
