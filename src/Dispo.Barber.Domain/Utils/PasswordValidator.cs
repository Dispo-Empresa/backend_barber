using System.Text.RegularExpressions;

namespace Dispo.Barber.Domain.Utils
{
    public static class PasswordValidator
    {
        public static void Validate(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("A senha não pode estar vazia.");

            if (password.Length < 7)
                throw new ArgumentException("A senha deve ter no mínimo 7 caracteres.");

            if (!Regex.IsMatch(password, @"[A-Z]"))
                throw new ArgumentException("A senha deve ter pelo menos uma letra maiúscula.");

            if (!Regex.IsMatch(password, @"[a-z]"))
                throw new ArgumentException("A senha deve ter pelo menos uma letra minúscula.");

            if (!Regex.IsMatch(password, @"\d"))
                throw new ArgumentException("A senha deve ter pelo menos um número.");

            if (!Regex.IsMatch(password, "[!@#$%^&*(),.?\":{ }|<>]"))
                throw new ArgumentException("A senha deve ter pelo menos um caractere especial.");
        }
    }
}
