using Dispo.Barber.Domain.DTOs.Authentication.Response;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Exceptions;
using Dispo.Barber.Domain.Factories;
using Dispo.Barber.Domain.Integration.SubscriptionClient.Models;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Domain.Services.Interfaces;

namespace Dispo.Barber.Domain.Services
{
    public class StoreSubscriptionService(IUserRepository userRepository,
                                               ISubscriptionValidatorFactory subscriptionFactory,
                                               IUserService userService) : IStoreSubscriptionService
    {
        public async Task<SubscriptionData> ValidateSubscriptionAsync(User user, DevicePlatform? currentPlataform, CancellationToken cancellationToken)
        {
            if (user.IsOwner())
            {
                return await ValidateOwnerSubscriptionAsync(user, currentPlataform, cancellationToken);
            }
            else
            {
                return await ValidateDependentSubscriptionAsync(user, cancellationToken);
            }
        }

        private async Task<SubscriptionData> ValidateOwnerSubscriptionAsync(User owner, DevicePlatform? currentPlataform, CancellationToken cancellationToken)
        {
            if (currentPlataform is not null && owner.Platform != currentPlataform)
            {
                return new SubscriptionData
                {
                    HasChangedPlataformError = true,
                };
            }

            return await ExecuteSubscriptionValidationAsync(owner, owner.PurchaseToken, cancellationToken);
        }

        private async Task<SubscriptionData> ValidateDependentSubscriptionAsync(User dependent, CancellationToken cancellationToken)
        {
            var ownerId = dependent.BusinessUnity?.Company.OwnerId;
            var owner = await userRepository.GetAsNoTrackingAsync(cancellationToken, ownerId.GetValueOrDefault()) ?? throw new NotFoundException("Proprietário não encontrado.");

            return await ExecuteSubscriptionValidationAsync(dependent, owner.PurchaseToken, cancellationToken);
        }

        private async Task<SubscriptionData> ExecuteSubscriptionValidationAsync(User user, string? purchaseToken, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(purchaseToken))
            {
                await HandleInvalidSubscription(user, new SubscriptionValidationResponse
                {
                    ExpirationDate = DateTime.MinValue,
                    Status = user.Platform == DevicePlatform.Android ? "SUBSCRIPTION_STATE_UNSPECIFIED" 
                                                                     : "", // UNSPECIFIED para Ios?
                }, cancellationToken);
            }

            var subscriptionValidator = subscriptionFactory.CreateValidator(user.Platform);
            var response = await subscriptionValidator.ValidateSubscriptionAsync(user.PurchaseToken, cancellationToken);

            if (response.IsSubscriptionValid)
            {
                return await HandleValidSubscription(user, response);
            }
            else
            {
                return await HandleInvalidSubscription(user, response, cancellationToken);
            }
        }

        private async Task<SubscriptionData> HandleValidSubscription(User user, SubscriptionValidationResponse subscriptionValidationResponse)
        {
            return await BuildSubscriptionData(user, subscriptionValidationResponse);
        }

        private async Task<SubscriptionData> HandleInvalidSubscription(User user, SubscriptionValidationResponse subscriptionValidationResponse, CancellationToken cancellationToken)
        {
            await userService.UpdateAllFromCompany(cancellationToken, user.BusinessUnity.CompanyId, UserStatus.PendingRenew);

            return await BuildSubscriptionData(user, subscriptionValidationResponse);
        }

        private async Task<SubscriptionData> BuildSubscriptionData(User user, SubscriptionValidationResponse response)
        {
            return await Task.FromResult(new SubscriptionData
            {
                ExpirationDate = response.ExpirationDate,
                Status = response.StatusEnum,
                HasChangedPlataformError = false,
                Platform = user.Platform,
            });
        }
    }
}
