using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Class to handle symmetrical encryption.
/// </summary>
public class SecurityHandler
{

    /// <summary>
    /// Encrypt the plaintext given.
    /// </summary>
    /// <param name="plaintext">Text to encrypt</param>
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

        byte[] result = PerformTransformation(inputBytes, encryptor);
        //Console.WriteLine("Encryption length is: "+ result.Length);
        return Convert.ToBase64String(result);
    }

    /// <summary>
    /// Decrypt the plaintext given.
    /// </summary>
    /// <param name="cyphertext">Text to decrypt.</param>
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

        return Encoding.UTF8.GetString(PerformTransformation(inputBytes, decryptor));
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
        //Console.WriteLine("Inputsize: "+ input.Length);
        cs.FlushFinalBlock();

        return ms.ToArray();
    }

    /// <summary>
    /// Combine the two encryption keys into one.
    /// </summary>
    /// <param name="a">Key a to combine.</param>
    /// <param name="b">Key b to combine.</param>
    /// <returns>Combined key of a and b.</returns>
    public static EncryptionKey CombineKeys(EncryptionKey a, EncryptionKey b)
    {
        //Console.WriteLine("Combined keys a: " + a.key.Length + " and iv: " + a.IV.Length);
        //Console.WriteLine("Combined keys b: " + b.key.Length + " and iv: " + b.IV.Length);
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

    public static byte[] GetHashedPW(string pw, byte[] salt)
    {

        HashAlgorithm algo = SHA512.Create();

        byte[] pwBytes = Encoding.UTF8.GetBytes(pw);

        byte[] combinedBytes = pwBytes.Concat(salt).ToArray();

        return algo.ComputeHash(combinedBytes);
    }
}
