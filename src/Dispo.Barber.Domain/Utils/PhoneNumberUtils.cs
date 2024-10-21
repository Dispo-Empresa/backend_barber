namespace Dispo.Barber.Domain.Utils
{
    public static class PhoneNumberUtils
    {
        public static string FormatPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                throw new ArgumentException("Número de telefone inválido.");

            // Remove todos os caracteres não numéricos
            var cleaned = new string(phoneNumber.Where(char.IsDigit).ToArray());

            // Se o número começar com o código do país '55' e tiver 12 ou 13 dígitos
            if (cleaned.StartsWith("55") && cleaned.Length >= 12)
            {
                // Se tiver 13 dígitos, já está completo com o '9'
                if (cleaned.Length == 13) return "+" + cleaned;

                // Se tiver 12 dígitos, adiciona o '9' após o código do país e DDD
                if (cleaned.Length == 12) return $"+55{cleaned.Substring(2).Insert(2, "9")}";
            }
            else
            {
                // Caso o número tenha 11 dígitos (com DDD)
                if (cleaned.Length == 11) return $"+55{cleaned}";

                // Se tiver 10 dígitos (sem o '9'), adicionamos o dígito '9' após o DDD
                if (cleaned.Length == 10) return $"+55{cleaned.Insert(2, "9")}";

                // Tratamento especial para o caso de DDD ser "55" (Rio Grande do Sul) sem o código do país
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
