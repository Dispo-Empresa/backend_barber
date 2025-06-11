using Dispo.Barber.Domain.Models;

namespace Dispo.Barber.Application.AppServices.Interfaces
{
    public interface IDashboardAppService
    {
        Task<Dashboard> BuildDashboardForUser(CancellationToken cancellationToken, long userId);
    }
}
