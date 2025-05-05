using Dispo.Barber.Domain.DTOs.Phone;
using Dispo.Barber.Domain.Exceptions;
using Dispo.Barber.Domain.Integration;
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
                if (cache.Get(request.Phone) != null)
                {
                    throw new BusinessException("Um código já foi enviado para esse número, aguarde alguns minutos e tente novamente.");
                }

                var codeRandom = new Random().Next(1000, 9999).ToString();
                await smsService.SendMessageAsync(request.Phone, $"Seu código de verificação do Aura é {codeRandom}. Use-o para continuar.");
                cache.Add(request.Phone, codeRandom);

                return Ok(new { message = "Enviamos o seu código de verificação. Dá uma olhadinha no seu SMS!" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não conseguimos enviar o SMS agora. Por favor, tente novamente em instantes." });
            }
        }

        [HttpGet]
        public IActionResult GetSmsCode([FromQuery] string phone, [FromQuery] string sms)
        {
            try
            {
                var smsInCache = cache.Get(phone) ?? throw new NotFoundException("O código expirou.");
                if (smsInCache != sms)
                {
                    throw new BusinessException("O código é diferente do enviado.");
                }

                // TODO: Isso pode ser um problema, alguém pode interceptar a requisição
                //       setando o "data" = "true".
                return Ok(new { data = smsInCache == sms });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
