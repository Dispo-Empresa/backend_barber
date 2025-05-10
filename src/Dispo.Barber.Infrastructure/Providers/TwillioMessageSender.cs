using Dispo.Barber.Application.AppServices;
using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Providers;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Dispo.Barber.Infrastructure.Providers
{
    public class TwillioMessageSender : ITwillioMessageSender
    {
        private readonly ILogger<TwillioMessageSender> _logger;
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _twilioPhoneNumber;
        private readonly string _twilioPhoneNumberWhats;

        public TwillioMessageSender(ILogger<TwillioMessageSender> logger)
        {
            _logger = logger;
            _accountSid = Environment.GetEnvironmentVariable("TWILLIO_ACCOUNT_SID") ?? "";
            _authToken = Environment.GetEnvironmentVariable("TWILLIO_AUTH_TOKEN") ?? "";
            _twilioPhoneNumber = Environment.GetEnvironmentVariable("TWILLIO_PHONE_NUMBER") ?? "";
            _twilioPhoneNumberWhats = Environment.GetEnvironmentVariable("TWILLIO_WHATSAPP_PHONE_NUMBER") ?? "";
        }

        public async Task SendSmsMessageAsync(string phone, string messageBody)
        {
            try
            {
                TwilioClient.Init(_accountSid, _authToken);
                await MessageResource.CreateAsync(
                    body: messageBody,
                    from: new PhoneNumber(_twilioPhoneNumber),
                    to: new PhoneNumber(phone)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending sms message to following phone: {ex.Message}");
                throw;
            }
        }

        public async Task SendWhatsAppMessageAsync(string phone, params string[] contentVariables)
        {
            try
            {
                TwilioClient.Init(_accountSid, _authToken);
                var a = await MessageResource.CreateAsync(
                    body: "appointment_confirmation",
                    from: new PhoneNumber($"whatsapp:{_twilioPhoneNumberWhats}"),
                    to: new PhoneNumber($"whatsapp:{phone}"),
                    contentVariables: BuildContentVariables(contentVariables),
                    contentSid: "HX8cfd612f60e6069fd6ddcd2ada2bb822"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending whatsapp message to following phone: {ex.Message}");
                throw;
            }
        }

        private  string BuildContentVariables(params string[] variables)
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
