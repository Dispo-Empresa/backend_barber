using System.Security.Cryptography;
using System.Text;

namespace Dispo.Barber.Domain.Utils
{
    public static class CryptoHelper
    {
        private static readonly string Key = "DispoAuraApp-2025-SECURE-KEY@963";
        private static readonly string IV = "AuraInitVector20";

        public static string Encrypt(string plainText)
        {
            try
            {
                using Aes aesAlg = Aes.Create();
                aesAlg.Key = Encoding.UTF8.GetBytes(Key);
                aesAlg.IV = Encoding.UTF8.GetBytes(IV);

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using MemoryStream ms = new();
                using CryptoStream cs = new(ms, encryptor, CryptoStreamMode.Write);
                using (StreamWriter sw = new(cs))
                    sw.Write(plainText);

                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception e)
            {
                throw e;
            }   
        }

        public static string Decrypt(string cipherText)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(Key);
            aesAlg.IV = Encoding.UTF8.GetBytes(IV);

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            byte[] buffer = Convert.FromBase64String(cipherText);

            using MemoryStream ms = new(buffer);
            using CryptoStream cs = new(ms, decryptor, CryptoStreamMode.Read);
            using StreamReader sr = new(cs);

            return sr.ReadToEnd();
        }
    }
}
