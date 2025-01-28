using Dispo.Barber.Application.Service.Interface;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Dispo.Barber.Domain.Utils;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Service
{
    public class SmsService : ISmsService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _twilioPhoneNumber;
        private readonly string _twilioPhoneNumberWhats;
        private const string CANCELLATION_URL_BASE = "https://example.com/cancelar-agendamento?appointmentId=";

        public SmsService(string accountSid, string authToken, string twilioPhoneNumber, string twilioPhoneNumberWhats)
        {
            _accountSid = accountSid;
            _authToken = authToken;
            _twilioPhoneNumber = twilioPhoneNumber;
            _twilioPhoneNumberWhats = twilioPhoneNumberWhats;
        }
        public string GenerateAppointmentMessage(Appointment appointment)
        {
            try
            {
                var idAppointment = appointment.Id;

                return $"Olá, {appointment.Customer.Name}! Seu agendamento foi confirmado para o dia {appointment.Date:dd/MM/yyyy}. " +
                       $"Se por algum motivo você precisar cancelar, clique no link abaixo:\n{CANCELLATION_URL_BASE}{idAppointment}\n" +
                       "Estamos à disposição para qualquer dúvida!";
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao gerar a mensagem de confirmação de agendamento.", ex);
            }
        }

        public async Task SendMessageAsync(string phoneNumberDestiny, string messageBody)
        {
            try
            {
                phoneNumberDestiny = StringUtils.FormatPhoneNumber(phoneNumberDestiny);

                TwilioClient.Init(_accountSid, _authToken);

                string numberTwilio = _twilioPhoneNumber;

                await MessageResource.CreateAsync(
                    to: new PhoneNumber(phoneNumberDestiny),
                    from: new PhoneNumber(numberTwilio),
                    body: messageBody
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao enviar {messageBody}. Para o numero {phoneNumberDestiny}. {ex.Message}");
            }
        }

    }
}
