using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enum;

namespace Dispo.Barber.Application.Service.Interface
{
    public interface INotificationService
    {
        Task NotifyAsync(CancellationToken cancellationToken, string token, string title, string body, Dictionary<string, string> data);
        Task NotifyAsync(CancellationToken cancellationToken, string token, string title, string body, NotificationType notificationType);
        string GenerateCreateAppointmentMessageApp(Appointment appointment);
        string GenerateCancelAppointmentMessageApp(Appointment appointment);
    }
}
