using Dispo.Barber.Domain.DTOs.Company;
using Dispo.Barber.Domain.DTOs.Hub;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Exceptions;
using Dispo.Barber.Domain.Integration.HubClient;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Domain.Services.Interfaces;
using Dispo.Barber.Domain.Utils;

namespace Dispo.Barber.Domain.Services
{
    public class LicenceManagementService(ICompanyRepository companyRepository,
                                          IUserRepository userRepository,
                                          IHubIntegration hubIntegration,
                                          IUserContext userContext,
                                          IStoreSubscriptionService storeSubscriptionService) : ILicenceManagementService
    {
        public async Task ChangeLicensePlan(long companyId, ChangeLicensePlanDTO changeLicensePlanDTO, CancellationToken cancellationToken)
        {
            var company = await GetCompanyAsync(companyId, cancellationToken);
            var user = await GetUserAsync(company.OwnerId!.Value, cancellationToken);

            ValidateUserPermissions(company, user);

            await ExecuteLicensePlanChangeAsync(companyId, user, changeLicensePlanDTO, cancellationToken);
        }

        private async Task<Company> GetCompanyAsync(long companyId, CancellationToken cancellationToken)
        {
            return await companyRepository.GetAsNoTrackingAsync(cancellationToken, companyId) ?? throw new NotFoundException("Barbearia não encontrada.");
        }

        private async Task<User> GetUserAsync(long userId, CancellationToken cancellationToken)
        {
            return await userRepository.GetByIdWithBusinessUnitiesAsync(cancellationToken, userId) ?? throw new NotFoundException("Usuário não encontrado.");
        }

        private void ValidateUserPermissions(Company company, User user)
        {
            var loggedUserId = userContext.GetLoggedUserId();

            if (company.OwnerId != loggedUserId)
            {
                throw new BusinessException("Usuário não é o responsável pela licença da barbearia.");
            }
            else if (user.Status != UserStatus.Active)
            {
                throw new BusinessException("Usuário não está ativo.");
            }
        }

        private async Task ExecuteLicensePlanChangeAsync(long companyId, User user, ChangeLicensePlanDTO changeLicensePlanDTO, CancellationToken cancellationToken)
        {
            switch (changeLicensePlanDTO.LicensePlan)
            {
                case LicensePlan.BarberPremium:
                    await HandleBarberPremiumPlanAsync(companyId, user, changeLicensePlanDTO, cancellationToken);
                    break;
                case LicensePlan.BarberPremiumTrial:
                    await HandleBarberPremiumTrialPlanAsync(companyId, cancellationToken);
                    break;
                case LicensePlan.BarberFree:
                    await HandleBarberFreePlanAsync(companyId, cancellationToken);
                    break;
                default:
                    throw new BusinessException("Plano de licença inválido.");
            }
        }

        private async Task HandleBarberPremiumPlanAsync(long companyId, User user, ChangeLicensePlanDTO changeLicensePlanDTO, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(changeLicensePlanDTO.PurchaseToken))
                throw new BusinessException("PurchaseToken não identificado para alteração da licença.");

            user.PurchaseToken = changeLicensePlanDTO.PurchaseToken;
            userRepository.Update(user);
            await userRepository.SaveChangesAsync(cancellationToken);

            var subscriptionData = await storeSubscriptionService.ValidateSubscriptionAsync(user, null, cancellationToken);

            if (!subscriptionData.IsSubscriptionValid)
                throw new BusinessException("Assinatura não concluída na loja.");

            var licenceHubRequest = new LicenseRequestDTO()
            {
                CompanyId = companyId,
                PlanType = LicensePlan.BarberPremium,
                Expiration = subscriptionData.ExpirationDate
            };

            await hubIntegration.CreateHubLicense(licenceHubRequest, cancellationToken);
        }

        private async Task HandleBarberPremiumTrialPlanAsync(long companyId, CancellationToken cancellationToken)
        {
            var licenceHubRequest = new LicenseRequestDTO()
            {
                CompanyId = companyId,
                PlanType = LicensePlan.BarberPremiumTrial,
                Expiration = LocalTime.Now.AddDays(7)
            };

            await hubIntegration.CreateHubLicense(licenceHubRequest, cancellationToken);
        }

        private async Task HandleBarberFreePlanAsync(long companyId, CancellationToken cancellationToken)
        {
            var licenceHubRequest = new LicenseRequestDTO()
            {
                CompanyId = companyId,
                PlanType = LicensePlan.BarberFree,
            };

            await hubIntegration.CreateHubLicense(licenceHubRequest, cancellationToken);
        }
    }
}
