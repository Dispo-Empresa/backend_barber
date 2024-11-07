using Dispo.Barber.Domain.Model;

namespace Dispo.Barber.Application.Service.Interface
{
    public interface IDashboardService
    {
        Task<Dashboard> BuildDashboardForUser(CancellationToken cancellationToken, long userId);
    }
}
