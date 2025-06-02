using Dispo.Barber.Domain.Integration;
using Google.Apis.AndroidPublisher.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;

namespace Dispo.Barber.Infrastructure.Integration
{
    public class SubscriptionValidationResult
    {
        public bool IsActive { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsInGracePeriod { get; set; }
        public bool IsTrial { get; set; }
        public string ErrorMessage { get; set; }
        public object RawData { get; set; }
    }

    public class SubscriptionIntegration : ISubscriptionIntegration
    {
        private readonly AndroidPublisherService _service;

        private const string PACKAGE_NAME = "com.dispo.aura";
        private const string SUBSCRIPTION_ID = "aura_premium";

        public SubscriptionIntegration()
        {
            //GoogleCredential credential;
            //string googleCloudCredentialsPath = Environment.GetEnvironmentVariable("AURA_GOOGLE_CLOUD_CREDENTIALS") ?? "";
            //using (var stream = new FileStream(googleCloudCredentialsPath, FileMode.Open, FileAccess.Read))
            //{
            //    credential = GoogleCredential.FromStream(stream)
            //        .CreateScoped(AndroidPublisherService.Scope.Androidpublisher);
            //}
            //
            //_service = new AndroidPublisherService(new BaseClientService.Initializer()
            //{
            //    HttpClientInitializer = credential,
            //    ApplicationName = "Aura Subscription Validator"
            //});
        }

        public async Task ValidateAndroidSubscriptionAsync(string token, CancellationToken cancellationToken)
        {
            var request = _service.Purchases.Subscriptionsv2.Get(PACKAGE_NAME, token);

            try
            {
                var purchase = await request.ExecuteAsync();

                //return new SubscriptionValidationResult
                //{
                //    IsActive = purchase.AutoRenewing == true &&
                //               purchase.ExpiryTimeMillis > DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                //    ExpiryDate = DateTimeOffset.FromUnixTimeMilliseconds(purchase.ExpiryTimeMillis ?? 0).UtcDateTime,
                //    IsCancelled = purchase.CancelReason != null && purchase.CancelReason != 0,
                //    IsInGracePeriod = purchase.PaymentState == 1, // 1 = Pending payment
                //    IsTrial = purchase.IntroductoryPriceInfo != null,
                //    RawData = purchase
                //};
            }
            catch (Google.GoogleApiException ex)
            {
                Console.WriteLine($"Google API Error: {ex.Message}");
                //return new SubscriptionValidationResult
                //{
                //    IsActive = false,
                //    ErrorMessage = ex.Message
                //};
            }
        }

        public async Task ValidateIosSubscriptionAsync(string receipt, CancellationToken cancellationToken)
        {

        }
    }
}
