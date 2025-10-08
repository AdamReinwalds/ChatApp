using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace ChatApp.Business.Helpers;

public class EncryptionHelper(IConfiguration configuration)
{
    private readonly IConfiguration _configuration = configuration;
    private readonly byte[] key = Convert.FromBase64String(configuration.GetSection("Crypto")["AesKeyBase64"]!);

    public string Encrypt(string plainText)
    {
        byte[] nonce = new byte[12];
        RandomNumberGenerator.Fill(nonce); 
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] cipherBytes = new byte[plainBytes.Length];
        byte[] tag = new byte[16];

        using var aes = new AesGcm(key, 16);

        aes.Encrypt(nonce, plainBytes, cipherBytes, tag);

        var output = new byte[nonce.Length + tag.Length + cipherBytes.Length];

        Buffer.BlockCopy(nonce, 0, output, 0, nonce.Length);
        Buffer.BlockCopy(tag, 0, output, nonce.Length, tag.Length);
        Buffer.BlockCopy(cipherBytes, 0, output, nonce.Length + tag.Length, cipherBytes.Length);

        return Convert.ToBase64String(output);

    }

    public string Decrypt(string encrypted)
    {
        var input = Convert.FromBase64String(encrypted);

        byte[] nonce = new byte[12];
        byte[] tag = new byte[16];
        byte[] cipherBytes = new byte[input.Length - nonce.Length - tag.Length];

        Buffer.BlockCopy(input, 0, nonce, 0, nonce.Length);
        Buffer.BlockCopy(input, nonce.Length, tag, 0, tag.Length);
        Buffer.BlockCopy(input, nonce.Length + tag.Length, cipherBytes, 0, cipherBytes.Length);

        byte[] plainBytes = new byte[cipherBytes.Length];

        using var aesGcm = new AesGcm(key, 16);
        aesGcm.Decrypt(nonce, cipherBytes, tag, plainBytes);

        return Encoding.UTF8.GetString(plainBytes);
    }
}
