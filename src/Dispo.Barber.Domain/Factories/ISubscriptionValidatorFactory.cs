using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Integration.SubscriptionClient;

namespace Dispo.Barber.Domain.Factories
{
    public interface ISubscriptionValidatorFactory
    {
        ISubscriptionValidator CreateValidator(DevicePlatform plataform);
    }
}