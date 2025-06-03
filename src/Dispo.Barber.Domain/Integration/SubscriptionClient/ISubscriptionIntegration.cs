using Dispo.Barber.Domain.Integration.SubscriptionClient.Models;

namespace Dispo.Barber.Domain.Integration.SubscriptionClient
{
    public interface ISubscriptionIntegration
    {
        Task<AndroidSubscriptionValidationResponse> GetAndroidSubscriptionAsync(string token, CancellationToken cancellationToken);
    }
}
