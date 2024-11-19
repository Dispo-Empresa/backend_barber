using Microsoft.AspNetCore.SignalR;

namespace Dispo.Barber.API.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string notification)
        {
            await Clients.All.SendAsync("Notify", notification);
        }
    }
}
