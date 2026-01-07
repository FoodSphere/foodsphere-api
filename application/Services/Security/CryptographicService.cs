using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;

namespace FoodSphere.Services;

public class AesServiceProvider : IDataProtectionProvider
{
    public IDataProtector CreateProtector(string purpose)
    {
        return new AesService(
            RandomNumberGenerator.GetBytes(AesService.KeySize),
            RandomNumberGenerator.GetBytes(AesService.KeySize)
        );
    }
}

public class AesService : IDataProtector
{
    readonly byte[] encryptionKey;
    readonly byte[] signingKey;
    public const int KeySize = 32; // 256 bit
    const int NonceSize = 12; // 96 bit
    const int TagSize = 16; // 128 bit

    // derive key from password (use a per-password random salt and store salt)
    // https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing
    public static byte[] DeriveKeyFrom(string password, byte[] salt, int iterations = 100_000)
    {
        if (salt is null || salt.Length < 16)
            throw new ArgumentException("Use a 16+ byte random salt.");

        // hash
        var pbkdf2 = Rfc2898DeriveBytes.Pbkdf2(
            password, salt, iterations, HashAlgorithmName.SHA256, KeySize);

        return pbkdf2;
    }

    public AesService(byte[] encryptionKey, byte[] signingKey)
    {
        this.encryptionKey = encryptionKey;
        this.signingKey = signingKey;
    }

    public IDataProtector CreateProtector(string purpose)
    {
        return this;
    }

    public byte[] Protect(byte[] plaintext)
    {
        if (encryptionKey.Length != KeySize)
            throw new ArgumentException($"encryption_key != {KeySize} bytes.");

        var nonce = RandomNumberGenerator.GetBytes(NonceSize);
        var tag = new byte[TagSize];
        var ciphertext = new byte[plaintext.Length];
        var combined = new byte[NonceSize + TagSize + ciphertext.Length];

        using var aes = new AesGcm(encryptionKey, TagSize);
        aes.Encrypt(nonce, plaintext, ciphertext, tag);

        // nonce || tag || ciphertext
        Buffer.BlockCopy(nonce, 0, combined, 0, NonceSize);
        Buffer.BlockCopy(tag, 0, combined, NonceSize, TagSize);
        Buffer.BlockCopy(ciphertext, 0, combined, NonceSize + TagSize, ciphertext.Length);

        return combined;
    }

    public byte[] Unprotect(byte[] combined)
    {
        var nonce = new byte[NonceSize];
        var tag = new byte[TagSize];
        var ciphertext = new byte[combined.Length - NonceSize - TagSize];
        var plaintext = new byte[ciphertext.Length];

        // nonce || tag || ciphertext
        Buffer.BlockCopy(combined, 0, nonce, 0, NonceSize);
        Buffer.BlockCopy(combined, NonceSize, tag, 0, TagSize);
        Buffer.BlockCopy(combined, NonceSize + TagSize, ciphertext, 0, ciphertext.Length);

        using var aes = new AesGcm(encryptionKey, TagSize);
        aes.Decrypt(nonce, ciphertext, tag, plaintext);

        return plaintext;
    }
}