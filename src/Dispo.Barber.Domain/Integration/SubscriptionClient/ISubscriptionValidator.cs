using Dispo.Barber.Domain.Integration.SubscriptionClient.Models;

namespace Dispo.Barber.Domain.Integration.SubscriptionClient
{
    public interface ISubscriptionValidator
    {
        Task<SubscriptionValidationResponse> ValidateSubscriptionAsync(string purchaseToken, CancellationToken cancellationToken);
    }
}
