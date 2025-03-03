using Dispo.Barber.Domain.Models;

namespace Dispo.Barber.Application.AppServices.Interface
{
    public interface IDashboardAppService
    {
        Task<Dashboard> BuildDashboardForUser(CancellationToken cancellationToken, long userId);
    }
}
