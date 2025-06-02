namespace Dispo.Barber.Domain.Integration
{
    public interface ISubscriptionIntegration
    {
        Task ValidateAndroidSubscriptionAsync(string token, CancellationToken cancellationToken);
    }
}
