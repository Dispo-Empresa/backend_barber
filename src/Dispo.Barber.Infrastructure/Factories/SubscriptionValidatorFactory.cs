using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Factories;
using Dispo.Barber.Domain.Integration.SubscriptionClient;
using Dispo.Barber.Infrastructure.Integration.SubscriptionClient;
using Microsoft.Extensions.DependencyInjection;

namespace Dispo.Barber.Infrastructure.Factories
{
    public class SubscriptionValidatorFactory(IServiceProvider serviceProvider) : ISubscriptionValidatorFactory
    {
        public ISubscriptionValidator CreateValidator(DevicePlatform platform)
        {
            return platform switch
            {
                DevicePlatform.Android => serviceProvider.GetRequiredService<GooglePlayStrategyValidator>(),
                DevicePlatform.Ios => serviceProvider.GetRequiredService<AppStoreStrategyValidator>(),
                _ => throw new NotSupportedException("Plataforma não suportada")
            };
        }
    }
}
