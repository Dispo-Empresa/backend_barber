using Dispo.Barber.Domain.Exceptions;
using Dispo.Barber.Domain.Integration.SubscriptionClient;
using Dispo.Barber.Domain.Integration.SubscriptionClient.Models;
using Google.Apis.AndroidPublisher.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;

namespace Dispo.Barber.Infrastructure.Integration.SubscriptionClient
{
    public class SubscriptionIntegration : ISubscriptionIntegration
    {
        private readonly AndroidPublisherService _service;

        private const string PACKAGE_NAME = "com.dispo.aura";
        private const string SUBSCRIPTION_ID = "aura_premium";

        public SubscriptionIntegration()
        {
            GoogleCredential credential;
            string googleCloudCredentialsPath = Environment.GetEnvironmentVariable("AURA_GOOGLE_CLOUD_CREDENTIALS") ?? "";
            using (var stream = new FileStream(googleCloudCredentialsPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(AndroidPublisherService.Scope.Androidpublisher);
            }
            
            _service = new AndroidPublisherService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Aura Subscription Validator"
            });
        }

        public async Task<AndroidSubscriptionValidationResponse> GetAndroidSubscriptionAsync(string token, CancellationToken cancellationToken)
        {
            var request = _service.Purchases.Subscriptionsv2.Get(PACKAGE_NAME, token);

            try
            {
                var purchaseResponse = await request.ExecuteAsync();

                return new AndroidSubscriptionValidationResponse
                {
                    StartDate = purchaseResponse.StartTimeDateTimeOffset.GetValueOrDefault().UtcDateTime,
                    ExpiryDate = purchaseResponse.LineItems.First().ExpiryTimeDateTimeOffset.GetValueOrDefault().UtcDateTime,
                    Status = purchaseResponse.SubscriptionState,
                };
            }
            catch (Google.GoogleApiException ex)
            {
                throw new BusinessException($"Google API Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao conectar com a API Google Play Developer API: {ex.Message}");
            }
        }

        public async Task ValidateIosSubscriptionAsync(string receipt, CancellationToken cancellationToken)
        {

        }
    }
}
