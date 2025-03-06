using Dispo.Barber.Domain.DTO.Hub;
using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Integration;
using Dispo.Barber.Domain.Utils;
using Newtonsoft.Json;
using RestSharp;

namespace Dispo.Barber.Infrastructure.Integration
{
    public class HubIntegration : IHubIntegration
    {
        public bool HubIntegrationEnabled
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(Environment.GetEnvironmentVariable("HUB_INTEGRATION_ENABLED"));
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public async Task<PlanType> GetPlanType(CancellationToken cancellationToken, long companyId)
        {
            try
            {
                if (!HubIntegrationEnabled)
                {
                    return PlanType.BarberFree;
                }

                var options = new RestClientOptions($"https://localhost:7074/v1/licenses/{companyId}/plan");
                var client = new RestClient(options);
                var request = new RestRequest();
                var response = await client.GetAsync(request, cancellationToken);

                if (response == null || response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return PlanType.BarberFree;
                }

                var plan = JsonConvert.DeserializeObject<PlanDTO>(response.Content);
                if (plan is null)
                {
                    return PlanType.BarberFree;
                }

                return plan.Name.ToEnum<PlanType>();
            }
            catch (Exception)
            {
                return PlanType.BarberFree;
            }
        }
    }
}
