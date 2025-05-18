namespace Dispo.Barber.Domain.Utils
{
    public static class StringUtils
    {
        public static string FormatPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return phoneNumber;

            var cleaned = new string(phoneNumber.Where(char.IsDigit).ToArray());

            if (cleaned.StartsWith("55") && cleaned.Length >= 12)
            {
                if (cleaned.Length == 13) return "+" + cleaned;

                if (cleaned.Length == 12) return $"+55{cleaned.Substring(2).Insert(2, "9")}";
            }
            else
            {
                if (cleaned.Length == 11) return $"+55{cleaned}";

                if (cleaned.Length == 10) return $"+55{cleaned.Insert(2, "9")}";

                if (cleaned.Length == 11 && cleaned.StartsWith("55"))
                {
                    return $"+55{cleaned}";
                }
                if (cleaned.Length == 10 && cleaned.StartsWith("55"))
                {
                    return $"+55{cleaned.Insert(2, "9")}";
                }
            }

            throw new ArgumentException("Número de telefone inválido.");
        }


    }
}
