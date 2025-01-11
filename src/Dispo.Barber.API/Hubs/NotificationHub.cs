using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.SignalR;

namespace Dispo.Barber.API.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string notification)
        {
            await Clients.All.SendAsync("Notify", notification);
        }


        public async Task SendNotificationAsync(string deviceToken, string title, string body)
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(Environment.GetEnvironmentVariable("BARBER_FIREBASE_ACCOUNT"))
                });
            }

            var message = new FirebaseAdmin.Messaging.Message()
            {
                Token = deviceToken,
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                },
                Data = new Dictionary<string, string>()
        {
            { "chave_personalizada", "valor" }
        }
            };

            string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            Console.WriteLine($"Notificação enviada com sucesso! ID: {response}");
        }

    }
}
