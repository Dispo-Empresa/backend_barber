using Dispo.Barber.Domain.DTOs.Hub;
using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Exceptions;
using Dispo.Barber.Domain.Integration;
using Dispo.Barber.Domain.Utils;
using Dispo.Barber.Domain.Utils.Extensions;
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

        public LicenceDTO BasicLicence
        {
            get
            {
                return new LicenceDTO
                {
                    Key = "FREE",
                    ExpirationDate = LocalTime.Now.AddYears(1),
                    Plan = new PlanDTO
                    {
                        Name = PlanType.BarberFree.ToString()
                    }
                };
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

                var url = Environment.GetEnvironmentVariable("HUB_INTEGRATION_URL");
                var options = new RestClientOptions($"{url}/v1/licenses/{companyId}/plan");
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

        public async Task<LicenceDTO> GetLicenceDetails(CancellationToken cancellationToken, long companyId)
        {
            try
            {
                if (!HubIntegrationEnabled)
                {
                    return BasicLicence;
                }

                var url = Environment.GetEnvironmentVariable("HUB_INTEGRATION_URL");
                var options = new RestClientOptions($"{url}/v1/licenses/{companyId}/full");
                var client = new RestClient(options);
                var request = new RestRequest();
                var response = await client.GetAsync(request, cancellationToken);

                if (response == null || response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return BasicLicence;
                }

                var licenceDetails = JsonConvert.DeserializeObject<LicenceDTO>(response.Content);
                if (licenceDetails is null)
                {
                    return BasicLicence;
                }

                return licenceDetails;
            }
            catch (Exception)
            {
                return BasicLicence;
            }
        }

        public async Task CreateHubLicence(LicenceRequestDTO licenceRequestDTO, CancellationToken cancellationToken)
        {
            try
            {
                if (!HubIntegrationEnabled)
                    return;

                var url = Environment.GetEnvironmentVariable("HUB_INTEGRATION_URL");
                var options = new RestClientOptions($"{url}/v1/licenses");
                var client = new RestClient(options);
                var request = new RestRequest().AddJsonBody(licenceRequestDTO);
                var response = await client.PostAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                    throw new BusinessException("Erro ao criar a licença no HUB");
            }
            catch (Exception ex)
            {
                throw new BusinessException("Erro ao conectar com a API do HUB");
            }
        }
    }
}
