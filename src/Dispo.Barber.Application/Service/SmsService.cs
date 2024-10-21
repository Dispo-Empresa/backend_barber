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
            var idAppointment = appointment.Id;

            return $"Olá, {appointment.Customer.Name}! Seu agendamento foi confirmado para o dia {appointment.Date:dd/MM/yyyy}. " +
                   $"Se por algum motivo você precisar cancelar, clique no link abaixo:\n{CANCELLATION_URL_BASE}{idAppointment}\n" +
                   "Estamos à disposição para qualquer dúvida!";
        }


        public async Task<string> SendMessageAsync(string phoneNumber, string messageBody, string messageType)
        {
            phoneNumber = PhoneNumberUtils.FormatPhoneNumber(phoneNumber);

            TwilioClient.Init(_accountSid, _authToken);

            var verificationCode = new Random().Next(1000, 9999).ToString();

            var fullMessageBody = string.IsNullOrEmpty(messageBody)
                ? $"Seu código de verificação é: {verificationCode}"
                : messageBody.Replace("{code}", verificationCode);

            string typeMsg = messageType == MessageType.WhatsApp ? _twilioPhoneNumberWhats : _twilioPhoneNumber;
            string toPhoneNumber = MessageType.ToPrefix(messageType) + phoneNumber;

            try
            {
                var message = await MessageResource.CreateAsync(
                    to: new PhoneNumber(toPhoneNumber),
                    from: new PhoneNumber(typeMsg),
                    body: fullMessageBody
                );

                return verificationCode;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Erro ao enviar {messageType}.", ex);
            }
        }


    }
}
