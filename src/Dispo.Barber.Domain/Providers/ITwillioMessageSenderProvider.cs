namespace Dispo.Barber.Domain.Providers
{
    public interface ITwillioMessageSenderProvider
    {
        Task SendSmsMessageAsync(string phone, string messageBody);
        Task SendWhatsAppMessageAsync(string phone, string template, string contentId, params string[] contentVariables);
        Task SendTokenVerificationWhatsAppMessage(string phone, string code);
    }
}
