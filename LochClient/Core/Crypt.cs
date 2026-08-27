using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace Loch.Core
{
    public class Crypt
    {
        private const int MinPasswordLength = 12;
        private const int MaxPasswordLength = 128;
        private const int SaltSizeBytes = 32;
        private const int IVSizeBytes = 16;
        private const int KeySizeBytes = 32;
        private const int PBKDF2Iterations = 100_000;

        public static byte[] DeriveKey(string password, byte[] salt)
        {
            ValidatePassword(password);

            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                iterations: PBKDF2Iterations,
                hashAlgorithm: HashAlgorithmName.SHA256
            );

            return pbkdf2.GetBytes(KeySizeBytes);
        }


        public  Crypt(string text, string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSizeBytes);

            byte[] Hkey = DeriveKey(password, salt);

            byte[] iv = RandomNumberGenerator.GetBytes(IVSizeBytes);
            RandomNumberGenerator.Fill(iv);

            byte[] data = Encoding.UTF8.GetBytes(text);
            byte[] encrypted = EncryptAes(data, Hkey, iv);

        }

        static byte[] EncryptAes(byte[] data, byte[] Hkey, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Hkey;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor())
                    return encryptor.TransformFinalBlock(data, 0, data.Length);
            }
        }

        public static string Decrypt(byte[] encryptedData, string password, byte[] salt, byte[] iv)
        {
            byte[] Hkey = DeriveKey(password, salt);
            byte[] decrypted = DecryptAes(encryptedData, Hkey, iv);
            return Encoding.UTF8.GetString(decrypted);
        }

        static byte[] DecryptAes(byte[] encryptedData, byte[] Hkey, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Hkey;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var decryptor = aes.CreateDecryptor())
                    return decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            }
        }

        public static void ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));

            if (password.Length < MinPasswordLength)
                throw new ArgumentException(
                    $"Password must be at least {MinPasswordLength} characters long.",
                    nameof(password)
                );

            if (password.Length > MaxPasswordLength)
                throw new ArgumentException(
                    $"Password cannot exceed {MaxPasswordLength} characters.",
                    nameof(password)
                );
        }
    }
}