namespace Dispo.Barber.Domain.Utils
{
    public static class MessageType
    {
        public const string Sms = "sms";
        public const string WhatsApp = "whatsapp";

        public static string ToPrefix(string messageType)
        {
            return messageType.ToLower() switch
            {
                "sms" => "",
                "whatsapp" => "whatsapp:",
                _ => throw new ArgumentOutOfRangeException(nameof(messageType), $"Tipo de mensagem inválido: {messageType}")
            };
        }
    }
}
