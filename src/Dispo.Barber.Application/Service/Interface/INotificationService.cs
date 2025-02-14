using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Service.Interface
{
    public interface INotificationService
    {
        Task NotifyAsync(CancellationToken cancellationToken, string token, string title, string body, Dictionary<string, string> data);
        Task NotifyAsync(CancellationToken cancellationToken, string token, string title, string body);
        string GenerateCreateAppointmentMessageApp(Appointment appointment);
        string GenerateCancelAppointmentMessageApp(Appointment appointment);
    }
}
