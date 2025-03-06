using Dispo.Barber.Domain.Enums;

namespace Dispo.Barber.Domain.Integration
{
    public interface IHubIntegration
    {
        Task<PlanType> GetPlanType(CancellationToken cancellationToken, long companyId);
    }
}
