namespace Dispo.Barber.Domain.Providers
{
    public interface ITwillioMessageSender
    {
        Task SendSmsMessageAsync(string phone, string messageBody);
        Task SendWhatsAppMessageAsync(string phone, params string[] contentVariables);
    }
}
