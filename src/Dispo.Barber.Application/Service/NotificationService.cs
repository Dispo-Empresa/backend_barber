using Dispo.Barber.Application.AppService;
using Dispo.Barber.Application.Service.Interface;
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
    }
}
