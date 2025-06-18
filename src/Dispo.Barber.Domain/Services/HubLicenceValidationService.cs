using Dispo.Barber.Domain.DTOs.Hub;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Integration.HubClient;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Domain.Services.Interfaces;

namespace Dispo.Barber.Domain.Services
{
    public class HubLicenceValidationService(IUserRepository userRepository,
                                             IHubIntegration hubIntegration,
                                             IUserService userService) : IHubLicenceValidationService
    {
        public async Task<LicenseDTO> GetOrCreateLicense(User user, CancellationToken cancellationToken)
        {
            var license = await hubIntegration.GetLicenseDetails(cancellationToken, user.BusinessUnity.CompanyId);
            if (license is not null)
            {
                if (license.Plan.IsFreePlan())
                    return license;

                if (license.IsExpired())
                {
                    await userService.UpdateAllFromCompany(cancellationToken, user.BusinessUnity.CompanyId, UserStatus.PendingRenew);
                }
                else
                {
                    await ActivateUsersIfPendingRenewal(user.BusinessUnity.CompanyId, cancellationToken);
                }

                return license;
            }

            await userService.UpdateAllFromCompany(cancellationToken, user.BusinessUnity.CompanyId, UserStatus.PendingRenew);

            return await hubIntegration.CreateHubLicense(new LicenseRequestDTO
            {
                CompanyId = user.BusinessUnity.CompanyId,
                PlanType = LicensePlan.BarberFree,
            }, cancellationToken);
        }

        private async Task ActivateUsersIfPendingRenewal(long companyId, CancellationToken cancellationToken)
        {
            if (!await userRepository.ExistsAsync(cancellationToken, w => w.BusinessUnity != null && w.BusinessUnity.CompanyId == companyId && w.Status == UserStatus.PendingRenew))
            {
                return;
            }

            await userService.UpdateAllFromCompany(cancellationToken, companyId, UserStatus.Active);
        }
    }
}
