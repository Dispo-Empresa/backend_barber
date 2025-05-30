using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Providers;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;

namespace Dispo.Barber.Infrastructure.Providers
{
    public class NotificationSenderProvider(ILogger<NotificationSenderProvider> logger) : INotificationSenderProvider
    {
        public async Task NotifyAsync(CancellationToken cancellationToken, string token, string title, string body, NotificationType notificationType)
        {
            try
            {
                var messageId = await FirebaseMessaging.DefaultInstance.SendAsync(new Message()
                {
                    Token = token,
                    Data = new Dictionary<string, string>()
                    {
                        ["Title"] = title,
                        ["Body"] = body,
                        ["NotificationType"] = notificationType.ToString("d")
                    },
                }, cancellationToken);

                logger.LogInformation("Mensagem com o ID {@ID} enviada para {@Token}.", messageId, token);
            }
            catch (Exception e)
            {
                logger.LogError("Erro ao notificar usu�rio {@Error}", e);
            }
        }

        public string GenerateCreateAppointmentMessageApp(Appointment appointment)
        {
            try
            {
                var clientName = appointment.Customer?.Name ?? "Cliente";
                var appointmentDate = appointment.Date.ToString("dd/MM/yyyy");

                return $"Novo agendamento confirmado para o cliente {clientName} no dia {appointmentDate}.";
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao gerar a mensagem de confirma��o de agendamento.");
            }
        }

        public string GenerateCancelAppointmentMessageApp(Appointment appointment)
        {
            try
            {
                var customerName = appointment.Customer?.Name ?? "Cliente";
                var appointmentDate = appointment.Date.ToString("dd/MM/yyyy");

                return $"Atencao! {customerName} cancelou o agendamento marcado para o dia {appointmentDate}.";
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao gerar a mensagem de cancelamento de agendamento.");
            }
        }
    }
}
