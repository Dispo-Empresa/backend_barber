using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Dispo.Barber.Domain.Utils;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Integration;

namespace Dispo.Barber.Infrastructure.Integration
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
        public string GenerateCreateAppointmentMessageSms(Appointment appointment)
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

        public string GenerateCancelAppointmentMessageSms(Appointment appointment)
        {
            try
            {
                var appointmentDate = appointment.Date.ToString("dd/MM/yyyy");

                return $"Olá, {appointment.Customer.Name}. Seu agendamento para o dia {appointmentDate} foi cancelado. " +
                       "Se isso foi um engano ou deseja reagendar, entre em contato conosco. " +
                       "Estamos à disposição para ajudá-lo!";
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao gerar a mensagem de cancelamento de agendamento.", ex);
            }
        }

        public async Task SendMessageAsync(string phone, string messageBody)
        {
            try
            {
                phone = StringUtils.FormatPhoneNumber(phone);
                TwilioClient.Init(_accountSid, _authToken);

                await MessageResource.CreateAsync(
                    to: new PhoneNumber(phone),
                    from: new PhoneNumber(_twilioPhoneNumber),
                    body: messageBody
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao enviar {messageBody}. Para o numero {phone}. {ex.Message}");
            }
        }

    }
}
