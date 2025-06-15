using Dispo.Barber.Domain.DTOs.Authentication.Response;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enums;

namespace Dispo.Barber.Domain.Services.Interfaces
{
    public interface ISubscriptionValidationService
    {
        Task<SubscriptionData> ValidateSubscriptionAsync(User user, DevicePlatform? currentPlataform, CancellationToken cancellationToken);
    }
}
