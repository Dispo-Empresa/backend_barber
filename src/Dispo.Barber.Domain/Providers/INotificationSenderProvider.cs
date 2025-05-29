using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enums;

namespace Dispo.Barber.Domain.Providers
{
    public interface INotificationSenderProvider
    {
        Task NotifyAsync(CancellationToken cancellationToken, string token, string title, string body, NotificationType notificationType);
        string GenerateCreateAppointmentMessageApp(Appointment appointment);
        string GenerateCancelAppointmentMessageApp(Appointment appointment);
    }
}
