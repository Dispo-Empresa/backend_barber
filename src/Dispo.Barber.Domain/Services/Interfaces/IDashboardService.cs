using Dispo.Barber.Domain.Models;

namespace Dispo.Barber.Domain.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<Dashboard> BuildDashboardForUser(CancellationToken cancellationToken, long userId);
    }
}
