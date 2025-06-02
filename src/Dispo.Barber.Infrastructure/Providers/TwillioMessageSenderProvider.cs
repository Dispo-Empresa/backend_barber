using Dispo.Barber.Domain.Exceptions;
using Dispo.Barber.Domain.Providers;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Dispo.Barber.Infrastructure.Providers
{
    public class TwillioMessageSenderProvider : ITwillioMessageSenderProvider
    {
        private readonly ILogger<TwillioMessageSenderProvider> _logger;
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _twilioPhoneNumberWhats;

        private const string TOKEN_VERIFICATION_CONTENT_SID = "HX6c6d390bceb79e306190675e00f3e25c";
        private const string TOKEN_VERIFICATION_TEMPLATE = "token_verification";

        public TwillioMessageSenderProvider(ILogger<TwillioMessageSenderProvider> logger)
        {
            _logger = logger;
            _accountSid = Environment.GetEnvironmentVariable("TWILLIO_ACCOUNT_SID") ?? "";
            _authToken = Environment.GetEnvironmentVariable("TWILLIO_AUTH_TOKEN") ?? "";
            _twilioPhoneNumberWhats = Environment.GetEnvironmentVariable("TWILLIO_WHATSAPP_NUMBER") ?? "";
        }

        public async Task SendWhatsAppMessageAsync(string phone, string template, string contentId, params string[] contentVariables)
        {
            try
            {
                TwilioClient.Init(_accountSid, _authToken);
                await MessageResource.CreateAsync(
                    body: template,
                    from: new PhoneNumber($"whatsapp:{_twilioPhoneNumberWhats}"),
                    to: new PhoneNumber($"whatsapp:{phone}"),
                    contentVariables: BuildContentVariables(contentVariables),
                    contentSid: contentId
                );
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending whatsapp message to following phone: {ex.Message}");
                throw;
            }
        }

        public async Task SendTokenVerificationWhatsAppMessage(string phone, string code)
        {
            try
            {
                TwilioClient.Init(_accountSid, _authToken);
                await MessageResource.CreateAsync(
                    body: TOKEN_VERIFICATION_TEMPLATE,
                    from: new PhoneNumber($"whatsapp:{_twilioPhoneNumberWhats}"),
                    to: new PhoneNumber($"whatsapp:{phone}"),
                    contentVariables: BuildContentVariables(code),
                    contentSid: TOKEN_VERIFICATION_CONTENT_SID
                );
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending whatsapp verification code to following phone: {ex.Message}");
                throw new BusinessException(ex.Message);
            }
        }

        private string BuildContentVariables(params string[] variables)
        {
            var dict = new Dictionary<string, string>();
            for (int i = 0; i < variables.Length; i++)
            {
                dict[(i + 1).ToString()] = variables[i];
            }

            return JsonSerializer.Serialize(dict, new JsonSerializerOptions
            {
                PropertyNamingPolicy = null
            });
        }
    }
}
