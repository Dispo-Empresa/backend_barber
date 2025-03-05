using Dispo.Barber.Application.AppService;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enums;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;

namespace Dispo.Barber.Application.Service
{
    public class NotificationService(ILogger<UserAppService> logger) : INotificationService
    {
        public async Task NotifyAsync(CancellationToken cancellationToken, string token, string title, string body, Dictionary<string, string> data)
        {
            var messageId = await FirebaseMessaging.DefaultInstance.SendAsync(new Message()
            {
                Token = token,
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                },
                Data = data
            }, cancellationToken);

            logger.LogInformation("Mensagem com o ID {@ID} enviada para {@Token}.", messageId, token);
        }

        public async Task NotifyAsync(CancellationToken cancellationToken, string token, string title, string body, NotificationType notificationType)
        {
            var messageId = await FirebaseMessaging.DefaultInstance.SendAsync(new Message()
            {
                Token = token,
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                },
                Data = new Dictionary<string, string>()
                {
                    ["NotificationType"] = notificationType.ToString("d")
                },
            }, cancellationToken);

            logger.LogInformation("Mensagem com o ID {@ID} enviada para {@Token}.", messageId, token);
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
                throw new Exception("Ocorreu um erro ao gerar a mensagem de confirmação de agendamento.");
            }
        }

        public string GenerateCancelAppointmentMessageApp(Appointment appointment)
        {
            try
            {
                var clientName = appointment.Customer?.Name ?? "Cliente";
                var appointmentDate = appointment.Date.ToString("dd/MM/yyyy");

                return $"Atenção! {clientName} cancelou o agendamento marcado para o dia {appointmentDate}.";
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao gerar a mensagem de cancelamento de agendamento.");
            }
        }

    }
}
