using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Text;

/// <summary>
/// Class to handle symmetrical encryption.
/// </summary>
public static class SecurityHandler
{

    /// <summary>
    /// Encrypt the plaintext given.
    /// </summary>
    /// <param name="plaintext">Text to encrypt (UTF8)</param>
    /// <param name="simKey">Key to use for encryption</param>
    /// <returns>The encrypted string.</returns>
    public static string Encrypt(string plaintext, EncryptionKey simKey)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(plaintext);

        Aes aes = Aes.Create();
        aes.KeySize = 256;
        aes.Key = simKey.key;
        aes.IV = simKey.IV;
        aes.Padding = PaddingMode.PKCS7;
        ICryptoTransform encryptor = aes.CreateEncryptor();

        return Convert.ToBase64String(PerformTransformation(inputBytes, encryptor));
    }

    /// <summary>
    /// Decrypt the plaintext given.
    /// </summary>
    /// <param name="cyphertext">Text to decrypt (Base64).</param>
    /// <param name="simKey">Key to use for decryption</param>
    /// <returns>The decrypted string.</returns>
    public static string Decrypt(string cyphertext, EncryptionKey simKey)
    {

        byte[] inputBytes = Convert.FromBase64String(cyphertext);

        Aes aes = Aes.Create();
        aes.KeySize = 256;
        aes.Key = simKey.key;
        aes.IV = simKey.IV;
        aes.Padding = PaddingMode.PKCS7;
        ICryptoTransform decryptor = aes.CreateDecryptor();

  
        byte[] decryptedBytes = PerformTransformation(inputBytes, decryptor);
        string result = Encoding.UTF8.GetString(decryptedBytes);
        return result;
        

    }

    /// <summary>
    /// Perform the transformation of the data.
    /// </summary>
    /// <param name="input">Data to perform transformation on.</param>
    /// <param name="tranformer">Transformer for the data. Specifies if the function will decrypt or encrypt.</param>
    /// <returns>Transformed data.</returns>
    private static byte[] PerformTransformation(byte[] input, ICryptoTransform tranformer)
    {
        MemoryStream ms = new MemoryStream();
        CryptoStream cs = new CryptoStream(ms, tranformer, CryptoStreamMode.Write);
        cs.Write(input, 0, input.Length);
        cs.FlushFinalBlock();
        byte[] result = ms.ToArray();
        return result;
    }


    /// <summary>
    /// Combine the two encryption keys into one.
    /// </summary>
    /// <param name="a">Key a to combine.</param>
    /// <param name="b">Key b to combine.</param>
    /// <returns>Combined key of a and b.</returns>
    public static EncryptionKey CombineKeys(EncryptionKey a, EncryptionKey b)
    {
        byte[] combinedKey = new byte[a.key.Length];
        byte[] combinedIV = new byte[b.IV.Length];

        for (int i = 0; i < a.key.Length; i++)
        {
            combinedKey[i] = (byte)(a.key[i] ^ b.key[i]);
        }

        for (int i = 0; i < b.IV.Length; i++)
        {
            combinedIV[i] = (byte)(a.IV[i] ^ b.IV[i]);
        }

        return new EncryptionKey(combinedKey, combinedIV);
    }
}
