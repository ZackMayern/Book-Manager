using System.Security.Cryptography;
using System.Text;

namespace EncryptionConsoleApp
{
    public static class EncryptionUtility
    {
        private static byte[] GetValidKey(string key)
        {
            // Ensure the key is exactly 16, 24, or 32 bytes long
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            if (keyBytes.Length < 16)
            {
                Array.Resize(ref keyBytes, 16);
            }
            else if (keyBytes.Length > 16 && keyBytes.Length < 24)
            {
                Array.Resize(ref keyBytes, 24);
            }
            else if (keyBytes.Length > 24 && keyBytes.Length < 32)
            {
                Array.Resize(ref keyBytes, 32);
            }
            else if (keyBytes.Length > 32)
            {
                Array.Resize(ref keyBytes, 32);
            }

            return keyBytes;
        }

        public static string EncryptString(string plainText, string key)
        {
            var keyBytes = GetValidKey(key);
            using var aes = Aes.Create();
            using var encryptor = aes.CreateEncryptor(keyBytes, aes.IV);

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }

            var iv = aes.IV;

            var encrypted = ms.ToArray();

            var result = new byte[iv.Length + encrypted.Length];
            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(encrypted, 0, result, iv.Length, encrypted.Length);

            return Convert.ToBase64String(result);
        }

        public static string DecryptString(string cipherText, string key)
        {
            var fullCipher = Convert.FromBase64String(cipherText);
            var iv = new byte[16];
            var cipher = new byte[fullCipher.Length - iv.Length];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            var keyBytes = GetValidKey(key);
            using var aes = Aes.Create();
            using var decryptor = aes.CreateDecryptor(keyBytes, iv);

            using var ms = new MemoryStream(cipher);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
        class Program
        {
            private const string EncryptionKey = ""; // Ensure this key is 16, 24, or 32 bytes long

            static void Main(string[] args)
            {
                Console.Write("Enter the plaintext string: ");
                string plainString = Console.ReadLine();

                string encryptedString = EncryptString(plainString, EncryptionKey);
                Console.WriteLine($"Encrypted  String: {encryptedString}");

                string decryptedString = DecryptString(encryptedString, EncryptionKey);
                Console.WriteLine($"Decrypted Connection String: {decryptedString}");
            }
        }
    }
}