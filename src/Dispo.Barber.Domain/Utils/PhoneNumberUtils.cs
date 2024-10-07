namespace Dispo.Barber.Domain.Utils
{
    public static class PhoneNumberUtils
    {
        public static string FormatPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                throw new ArgumentException("Número de telefone inválido.");

            var cleaned = new string(phoneNumber.Where(char.IsDigit).ToArray());

            if (cleaned.StartsWith("55"))
            {
                if (cleaned.Length == 13) return "+" + cleaned;
                if (cleaned.Length == 12) return $"+55{cleaned.Insert(3, "9")}"; 
            }
            else
            {
                if (cleaned.Length == 11) return $"+55{cleaned}";
                if (cleaned.Length == 10) return $"+55{cleaned.Insert(3, "9")}";
            }

            throw new ArgumentException("Número de telefone inválido.");
        }

    }
}
