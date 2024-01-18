using PhiThanh.Core.Utils;
using System.Security.Cryptography;
using System.Text;

namespace PhiThanh.Core.Extensions
{
    public static class HashExtensions
    {
        public static string HashMD5(this string text)
        {
            byte[] hash = MD5.HashData(Encoding.UTF8.GetBytes(text));
            StringBuilder sb = new();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }

            return sb.ToString();
        }

        public static string HashSHA256(this string text)
        {
            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(text));
            StringBuilder sb = new();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }

            return sb.ToString();
        }

        public static string Pbkdf2_SHA256(this string password, string salt, int interactionCount)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(salt);
            using Rfc2898DeriveBytes rfc2898DeriveBytes = new(password, bytes, interactionCount, HashAlgorithmName.SHA256);
            byte[] bytes2 = rfc2898DeriveBytes.GetBytes(32);
            return Convert.ToBase64String(bytes2);
        }

        public static string ToHashPassword(this string password)
        {
            var salt = StringUtils.RandomString(12);
            return $"pbkdf2_sha256${12000}${salt}${password.Pbkdf2_SHA256(salt, 12000)}";
        }
    }
}
