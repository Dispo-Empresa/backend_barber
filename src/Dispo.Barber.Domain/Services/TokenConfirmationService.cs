using Dispo.Barber.Domain.Cache;
using Dispo.Barber.Domain.Exceptions;
using Dispo.Barber.Domain.Providers;
using Dispo.Barber.Domain.Services.Interfaces;

namespace Dispo.Barber.Domain.Services
{
    public class TokenConfirmationService(ICacheManager cache, ITwillioMessageSenderProvider twillioMessageSenderProvider) : ITokenConfirmationService
    {
        private const int MAX_RETRIES = 3;

        public async Task GenerateCodeConfirmation(string phone)
        {
            if (cache.Get(phone) != null)
                throw new BusinessException("Um código já foi enviado para esse número, aguarde alguns minutos e tente novamente.");

            var codeRandom = new Random().Next(1000, 9999).ToString();
            cache.Add(phone, codeRandom);
            await twillioMessageSenderProvider.SendTokenVerificationWhatsAppMessage(phone, codeRandom);
        }

        public async Task<bool> ValidateCodeConfirmation(string phone, string sms)
        {
            var smsInCache = cache.Get(phone) ?? throw new NotFoundException("O código expirou.");

            if (smsInCache != sms)
            {
                CheckRetries(phone);
                throw new BusinessException("O código é diferente do enviado.");
            }

            InvalidateCode(phone);

            return await Task.FromResult(smsInCache == sms);
        }

        private void CheckRetries(string phone)
        {
            var key = $"{phone}-retries";
            var value = cache.Get(key);

            if (value != null)
            {
                var retries = Convert.ToInt32(value);
                retries++;

                if (retries >= MAX_RETRIES)
                    throw new BusinessException("Máximo de tentativas atingidas para o número, aguarde alguns minutos e tente novamente.");

                cache.Remove(key);
                cache.Add(key, retries.ToString());
            }
            else
            {
                cache.Add(key, "1");
            }
        }

        private void InvalidateCode(string phone)
        {
            cache.Remove(phone);
            cache.Remove($"{phone}-retries");
        }
    }
}
