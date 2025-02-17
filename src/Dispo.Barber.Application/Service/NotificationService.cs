using Dispo.Barber.Application.AppService;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.Entities;
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

        public async Task NotifyAsync(CancellationToken cancellationToken, string token, string title, string body)
        {
            var messageId = await FirebaseMessaging.DefaultInstance.SendAsync(new Message()
            {
                Token = token,
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
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

                return $"Olá! Um novo agendamento foi confirmado para o cliente {clientName} no dia {appointmentDate}. Consulte os detalhes no sistema.";
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

                return $"Atenção! O agendamento do cliente {clientName}, marcado para o dia {appointmentDate}, foi cancelado. Consulte os detalhes no sistema.";
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao gerar a mensagem de cancelamento de agendamento.");
            }
        }

    }
}
