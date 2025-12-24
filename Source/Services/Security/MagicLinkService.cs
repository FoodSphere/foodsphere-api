using System.Text;
using System.Security.Cryptography;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using MessagePack;

namespace FoodSphere.Services;

public interface IMagicLinkService
{
    public Task<string> Generate<TPayload>(TPayload payload) where TPayload : class;
    public Task<MagicLinkResult<TPayload>> Validate<TPayload>(string orderingString) where TPayload : class;
}

public class MagicLinkResult<TPayload>
{
    public bool IsValid { get; }
    public TPayload? Payload { get; }

    public MagicLinkResult()
    {
        IsValid = false;
        Payload = default;
    }

    public MagicLinkResult(TPayload? payload)
    {
        IsValid = true;
        Payload = payload;
    }
}

// https://www.scottbrady.io/c-sharp/json-web-encryption-jwe-in-dotnet-core
class JweMagicLinkService : IMagicLinkService
{
    readonly SymmetricSecurityKey EncryptionKey;
    readonly SymmetricSecurityKey SigningKey;
    readonly string Issuer;
    readonly string Audience;
    const int KeySize = 32; // can be other than 32 bytes?

    public JweMagicLinkService(IConfiguration config)
    {
        Issuer = config["Domain:api"]!;
        Audience = config["Domain:ordering"]!;

        SigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(config["MagicLink:encryption_key"]!));

        EncryptionKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(config["MagicLink:signing_key"]!));

        if (SigningKey.KeySize != KeySize)
            throw new ArgumentException($"MagicLink:signing_key != {KeySize} bytes.");

        if (EncryptionKey.KeySize != KeySize)
            throw new ArgumentException($"MagicLink:encryption_key != {KeySize} bytes.");
    }

    Dictionary<string, object> SerializePayload(object? payload)
    {
        throw new NotImplementedException();
    }

    TPayload DeserializePayload<TPayload>(IDictionary<string, object> claims)
    {
        throw new NotImplementedException();
    }

    public async Task<string> Generate<TPayload>(TPayload payload) where TPayload : class
    {
        var handler = new JsonWebTokenHandler();
        var token = handler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = Issuer,
            Audience = Audience,
            Claims = SerializePayload(payload),
            Expires = DateTime.UtcNow.AddMinutes(300),
            SigningCredentials = new SigningCredentials(
                SigningKey, SecurityAlgorithms.HmacSha256
            ),
            EncryptingCredentials = new EncryptingCredentials(
                EncryptionKey, SecurityAlgorithms.Aes256KW, SecurityAlgorithms.Aes256CbcHmacSha512
            ),
        });

        return token;
    }

    public async Task<MagicLinkResult<TPayload>> Validate<TPayload>(string orderingString) where TPayload : class
    {
        var handler = new JsonWebTokenHandler();
        var result = await handler.ValidateTokenAsync(
            orderingString, new TokenValidationParameters
            {
                ValidIssuer = Issuer,
                ValidAudience = Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = SigningKey,
                TokenDecryptionKey = EncryptionKey
            });

        var payload = DeserializePayload<TPayload>(result.Claims);

        return new(payload);
    }
}

/// ASP.NET Core API
/// <see cref="IPersistedDataProtector"/>
/// <see cref="IDataProtector"/>
/// <see cref="ITimeLimitedDataProtector"/>
/// <see cref="ISecureDataFormat{TData}"/> what serializer it use?
public class MessagePackMagicLinkService(IDataProtectionProvider provider) : IMagicLinkService
{
    readonly ITimeLimitedDataProtector _protector = provider.CreateProtector(nameof(MessagePackMagicLinkService)).ToTimeLimitedDataProtector();

    public async Task<string> Generate<TPayload>(TPayload payload) where TPayload : class
    {
        var serialized = MessagePackSerializer.Serialize(payload);
        var protectedSerialized = _protector.Protect(serialized, TimeSpan.FromMinutes(300));

        return Convert.ToBase64String(protectedSerialized);
    }

    public async Task<MagicLinkResult<TPayload>> Validate<TPayload>(string pathSegment) where TPayload : class
    {
        byte[] serialized;
        TPayload payload;
        var protectedSerialized = Convert.FromBase64String(pathSegment);

        try
        {
            serialized = _protector.Unprotect(protectedSerialized);
        }
        catch (CryptographicException)
        {
            return new();
        }

        try
        {
            payload = MessagePackSerializer.Deserialize<TPayload>(serialized);
        }
        catch (MessagePackSerializationException)
        {
            return new();
        }

        return new(payload);
    }
}

public class ProtoBufMagicLinkService() : IMagicLinkService
{
    public async Task<string> Generate<TPayload>(TPayload payload) where TPayload : class
    {
        throw new NotImplementedException();
    }

    public async Task<MagicLinkResult<TPayload>> Validate<TPayload>(string orderingString) where TPayload : class
    {
        throw new NotImplementedException();
    }
}

public class StatefulMagicLinkService(AppDbContext context) : IMagicLinkService
{
    public async Task<string> Generate<TPayload>(TPayload payload) where TPayload : class
    {
        await context.Set<TPayload>().AddAsync(payload);
        await context.SaveChangesAsync();

        return payload.ToString()!;
    }

    public async Task<MagicLinkResult<TPayload>> Validate<TPayload>(string orderingString) where TPayload : class
    {
        var entity = await context.Set<TPayload>().FindAsync(orderingString);

        if (entity is null)
        {
            return new();
        }

        return new(entity);
    }
}