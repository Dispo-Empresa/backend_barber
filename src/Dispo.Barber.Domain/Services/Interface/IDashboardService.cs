using Dispo.Barber.Domain.Models;

namespace Dispo.Barber.Domain.Services.Interface
{
    public interface IDashboardService
    {
        Task<Dashboard> BuildDashboardForUser(CancellationToken cancellationToken, long userId);
    }
}
