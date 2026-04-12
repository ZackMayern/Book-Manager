using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace Back.Domain.Helpers;

public class CryptoUtility
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

    public static string DecryptString(string cipherText, string key)
    {
        byte[] fullCipher = Convert.FromBase64String(cipherText);
        if (fullCipher.Length < 16) throw new ArgumentException("Invalid cipher text");

        byte[] iv = new byte[16];
        byte[] cipher = new byte[fullCipher.Length - iv.Length];

        Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

        byte[] keyBytes = GetValidKey(key);
        using Aes aes = Aes.Create();
        using ICryptoTransform decryptor = aes.CreateDecryptor(keyBytes, iv);

        using MemoryStream ms = new(cipher);
        using CryptoStream cs = new(ms, decryptor, CryptoStreamMode.Read);
        using StreamReader sr = new(cs);
        return sr.ReadToEnd();
    }

    public static RSA LoadPrivateKey(string pem)
    {
        using var reader = new StringReader(pem);
        var pemReader = new PemReader(reader);
        var keyPair = (AsymmetricCipherKeyPair)pemReader.ReadObject();
        var rsaParams = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)keyPair.Private);
        var rsa = RSA.Create();
        rsa.ImportParameters(rsaParams);
        return rsa;
    }
}