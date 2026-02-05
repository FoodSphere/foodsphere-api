using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace FoodSphere.Common.Services;

public class OrderingAuthService(
    ILogger<OrderingAuthService> logger,
    IOptions<EnvDomainApi> envDomainApi,
    IOptions<EnvDomainOrdering> envDomainOrdering
) {
    readonly EnvDomainApi envDomainApi = envDomainApi.Value;
    readonly EnvDomainOrdering envDomainOrdering = envDomainOrdering.Value;

    async Task<ClaimsIdentity> GetSubject(BillMember member)
    {
        List<Claim> claims = [];

        var consumerId = member.ConsumerId;

        if (consumerId is not null)
        {
            claims.Add(new(FoodSphereClaimType.Identity.UserIdClaimType, consumerId.ToString()!));
        }

        return new ClaimsIdentity(claims);
    }

    async Task<Dictionary<string, object>> GetClaims(BillMember member)
    {
        var claims = new Dictionary<string, object>
        {
            [FoodSphereClaimType.BillClaimType] = member.BillId,
            [FoodSphereClaimType.BillMemberClaimType] = (int)member.Id // or use .ToString()
        };

        return claims;
    }

    async Task<SecurityTokenDescriptor> GetTokenDescriptor(BillMember member)
    {
        // make expire
        return new SecurityTokenDescriptor
        {
            Issuer = envDomainApi.hostname,
            Audience = envDomainOrdering.hostname,
            Subject = await GetSubject(member),
            Claims = await GetClaims(member),
            Expires = DateTime.UtcNow.AddMinutes(300),
            SigningCredentials = envDomainOrdering.GetSigningCredentials(),
        };
    }

    public async Task<string> GenerateToken(BillMember member)
    {
        var handler = new JsonWebTokenHandler();
        var tokenDescriptor = await GetTokenDescriptor(member);
        var token = handler.CreateToken(tokenDescriptor);

        return token;
    }
}