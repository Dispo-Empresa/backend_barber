using Dispo.Barber.Domain.Integration.SubscriptionClient;
using Dispo.Barber.Domain.Integration.SubscriptionClient.Models;

namespace Dispo.Barber.Infrastructure.Integration.SubscriptionClient
{
    public class AppStoreStrategyValidator : ISubscriptionValidator
    {
        public Task<SubscriptionValidationResponse> ValidateSubscriptionAsync(string purchaseToken, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
