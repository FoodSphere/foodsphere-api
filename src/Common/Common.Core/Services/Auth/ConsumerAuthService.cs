using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace FoodSphere.Common.Services;

public class ConsumerAuthService(
    ILogger<ConsumerAuthService> logger,
    IOptions<EnvDomainApi> envDomainApi,
    IOptions<EnvDomainConsumer> envDomainConsumer
) {
    readonly EnvDomainApi envDomainApi = envDomainApi.Value;
    readonly EnvDomainConsumer envDomainConsumer = envDomainConsumer.Value;

    async Task<ClaimsIdentity> GetSubject(ConsumerUser user)
    {
        List<Claim> claims = [
            new(FoodSphereClaimType.Identity.UserIdClaimType, user.Id.ToString()),
            // new(AppClaimTypes.Identity.SecurityStampClaimType, user.SecurityStamp ?? string.Empty),
        ];

        return new ClaimsIdentity(claims);
    }

    async Task<Dictionary<string, object>> GetClaims(ConsumerUser user)
    {
        var claims = new Dictionary<string, object>()
        {
        };

        return claims;
    }

    async Task<SecurityTokenDescriptor> GetTokenDescriptor(ConsumerUser user)
    {
        return new SecurityTokenDescriptor
        {
            Issuer = envDomainApi.url,
            Audience = envDomainConsumer.url,
            Subject = await GetSubject(user),
            Claims = await GetClaims(user),
            Expires = DateTime.UtcNow.AddMinutes(300),
            SigningCredentials = envDomainConsumer.GetSigningCredentials(),
        };
    }

    async Task<TokenValidationParameters> GetValidationParameters()
    {
        return new TokenValidationParameters()
        {
            ValidIssuer = envDomainApi.url,
            // ValidAudience = envDomainConsumer.url,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = envDomainConsumer.GetSecurityKey(),
        };
    }

    public async Task<IDictionary<string, object>?> ValidateToken(string token)
    {
        var handler = new JsonWebTokenHandler();
        var validationParameters = await GetValidationParameters();
        var result = await handler.ValidateTokenAsync(token, validationParameters);

        if (!result.IsValid)
        {
            return null;
        }

        return result.Claims;
    }

    public async Task<string> GenerateToken(ConsumerUser user)
    {
        var handler = new JsonWebTokenHandler();
        var tokenDescriptor = await GetTokenDescriptor(user);
        var token = handler.CreateToken(tokenDescriptor);

        return token;
    }

    public async Task<bool> IsTwoFactorEnabledAsync(ConsumerUser user)
    {
        return user.TwoFactorEnabled;
    }
}
