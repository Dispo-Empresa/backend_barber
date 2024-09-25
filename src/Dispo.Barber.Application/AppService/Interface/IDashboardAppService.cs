using Dispo.Barber.Domain.Model;

namespace Dispo.Barber.Application.AppService.Interface
{
    public interface IDashboardAppService
    {
        Task<Dashboard> BuildDashboardForUser(long userId);
    }
}
