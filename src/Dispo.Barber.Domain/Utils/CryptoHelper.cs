using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;

namespace Dispo.Barber.Domain.Utils
{
    public static class CryptoHelper
    {
        private static string ToBase64Url(string base64)
        {
            return base64.Replace("+", "-").Replace("/", "_").Replace("=", "");
        }

        private static string FromBase64Url(string base64Url)
        {
            string base64 = base64Url.Replace("-", "+").Replace("_", "/");
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return base64;
        }

        public static string Encrypt(string plainText)
        {
            using Aes aesAlg = Aes.Create();

            string Key = Environment.GetEnvironmentVariable("ENCRYPTION_KEY") ?? throw new InvalidOperationException("Missing ENCRYPTION_KEY env variable");
            string IV = Environment.GetEnvironmentVariable("ENCRYPTION_IV") ?? throw new InvalidOperationException("Missing ENCRYPTION_IV env variable");

            aesAlg.Key = Encoding.UTF8.GetBytes(Key);
            aesAlg.IV = Encoding.UTF8.GetBytes(IV);

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream ms = new();
            using CryptoStream cs = new(ms, encryptor, CryptoStreamMode.Write);
            using (StreamWriter sw = new(cs))
                sw.Write(plainText);

            var base64 = Convert.ToBase64String(ms.ToArray());
            return ToBase64Url(base64); 
        }

        public static string Decrypt(string cipherText)
        {
            using Aes aesAlg = Aes.Create();

            string Key = Environment.GetEnvironmentVariable("ENCRYPTION_KEY") ?? throw new InvalidOperationException("Missing ENCRYPTION_KEY env variable");
            string IV = Environment.GetEnvironmentVariable("ENCRYPTION_IV") ?? throw new InvalidOperationException("Missing ENCRYPTION_IV env variable");

            aesAlg.Key = Encoding.UTF8.GetBytes(Key);
            aesAlg.IV = Encoding.UTF8.GetBytes(IV);

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            byte[] buffer = Convert.FromBase64String(FromBase64Url(cipherText));

            using MemoryStream ms = new(buffer);
            using CryptoStream cs = new(ms, decryptor, CryptoStreamMode.Read);
            using StreamReader sr = new(cs);

            return sr.ReadToEnd();
        }
    }
}
