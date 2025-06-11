using Dispo.Barber.Application.AppServices.Interfaces;
using Dispo.Barber.Domain.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Dispo.Barber.Application.AppServices
{
    public class TokenConfirmationAppService(ILogger<TokenConfirmationAppService> _logger, ITokenConfirmationService tokenConfirmationService) : ITokenConfirmationAppService
    {
        public async Task GenerateCodeConfirmation(string phone)
        {
            try
            {
                await tokenConfirmationService.GenerateCodeConfirmation(phone);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error generating confirmation token");
                throw;
            }
        }

        public async Task<bool> ValidateCodeConfirmation(string phone, string sms)
        {
            try
            {
                return await tokenConfirmationService.ValidateCodeConfirmation(phone, sms);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error validating confirmation token");
                throw;
            }
        }
    }
}
