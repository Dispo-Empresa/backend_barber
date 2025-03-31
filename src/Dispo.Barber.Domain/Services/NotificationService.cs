using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Services.Interface;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Dispo.Barber.Domain.Services
{
    public class NotificationService(ILogger<NotificationService> logger) : INotificationService
    {
        public async Task NotifyAsync(CancellationToken cancellationToken, string token, string title, string body, Dictionary<string, string> data)
        {
            try
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
            catch (Exception e)
            {
                logger.LogError("Erro ao notificar usuário {@Error}", e);
            }
        }

        public async Task NotifyAsync(CancellationToken cancellationToken, string token, string title, string body, NotificationType notificationType)
        {
            try
            {
                var messageId = await FirebaseMessaging.DefaultInstance.SendAsync(new Message()
                {
                    Token = token,
                    Notification = new Notification()
                    {
                        Title = Convert.ToBase64String(Encoding.UTF8.GetBytes(title)),
                        Body = Convert.ToBase64String(Encoding.UTF8.GetBytes(body))
                    },
                    Data = new Dictionary<string, string>()
                    {
                        ["NotificationType"] = notificationType.ToString("d")
                    },
                }, cancellationToken);

                logger.LogInformation("Mensagem com o ID {@ID} enviada para {@Token}.", messageId, token);
            }
            catch (Exception e)
            {
                logger.LogError("Erro ao notificar usuário {@Error}", e);
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
                throw new Exception("Ocorreu um erro ao gerar a mensagem de confirmação de agendamento.");
            }
        }

        public string GenerateCancelAppointmentMessageApp(Appointment appointment)
        {
            try
            {
                var customerName = appointment.Customer?.Name ?? "Cliente";
                var appointmentDate = appointment.Date.ToString("dd/MM/yyyy");

                return $"Atenção! {customerName} cancelou o agendamento marcado para o dia {appointmentDate}.";
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao gerar a mensagem de cancelamento de agendamento.");
            }
        }
    }
}
