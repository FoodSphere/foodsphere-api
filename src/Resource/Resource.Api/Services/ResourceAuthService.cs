using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;

namespace FoodSphere.Resource.Api.Services;

public class ResourceAuthService(
    ILogger<ResourceAuthService> logger,
    IOptions<EnvDomainApi> envDomainApi,
    IOptions<EnvDomainResource> envDomainResource
) {
    readonly EnvDomainApi envDomainApi = envDomainApi.Value;
    readonly EnvDomainResource envDomainResource = envDomainResource.Value;

    async Task<ClaimsIdentity> GetSubject(string identifier)
    {
        List<Claim> claims = [
            new Claim(FoodSphereClaimType.Identity.UserIdClaimType, identifier),
        ];

        return new ClaimsIdentity(claims);
    }

    async Task<Dictionary<string, object>> GetClaims(string identifier)
    {
        var claims = new Dictionary<string, object>()
        {
        };

        return claims;
    }

    async Task<SecurityTokenDescriptor> GetTokenDescriptor(string identifier)
    {
        return new SecurityTokenDescriptor
        {
            Issuer = envDomainApi.url,
            Audience = envDomainResource.url,
            Subject = await GetSubject(identifier),
            Claims = await GetClaims(identifier),
            Expires = DateTime.UtcNow.AddMinutes(300),
            SigningCredentials = envDomainResource.GetSigningCredentials(),
        };
    }

    public async Task<string> GenerateToken(string identifier)
    {
        var handler = new JsonWebTokenHandler();
        var tokenDescriptor = await GetTokenDescriptor(identifier);
        var token = handler.CreateToken(tokenDescriptor);

        return token;
    }
}