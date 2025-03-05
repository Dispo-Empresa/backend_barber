using Dispo.Barber.Domain.Enums;

namespace Dispo.Barber.Application.Integration
{
    public interface IHubIntegration
    {
        Task<PlanType> GetPlanType(CancellationToken cancellationToken, long companyId);
    }
}
