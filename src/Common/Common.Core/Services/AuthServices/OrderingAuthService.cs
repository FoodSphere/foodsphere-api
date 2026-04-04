using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace FoodSphere.Common.Service;

public class OrderingAuthService(
    ILogger<OrderingAuthService> logger,
    IOptions<EnvDomainApi> envDomainApi,
    IOptions<EnvDomainOrdering> envDomainOrdering,
    BillMemberRepository memberRepository)
{
    readonly EnvDomainApi envDomainApi = envDomainApi.Value;
    readonly EnvDomainOrdering envDomainOrdering = envDomainOrdering.Value;

    async Task<ClaimsIdentity> GetSubject(BillMemberKey memberKey)
    {
        List<Claim> claims = [];

        var member = await memberRepository.GetMember(memberKey);
        var consumerId = member?.ConsumerId;

        if (consumerId is not null)
            claims.Add(new(FoodSphereClaimType.Identity.UserIdClaimType, consumerId.ToString()!));

        return new ClaimsIdentity(claims);
    }

    async Task<Dictionary<string, object>> GetClaims(BillMemberKey memberKey)
    {
        var claims = new Dictionary<string, object>
        {
            [FoodSphereClaimType.BillClaimType] = memberKey.BillId,
            [FoodSphereClaimType.BillMemberClaimType] = (int)memberKey.Id // or use .ToString()
        };

        return claims;
    }

    async Task<SecurityTokenDescriptor> GetTokenDescriptor(BillMemberKey memberKey)
    {
        // make expire
        return new SecurityTokenDescriptor
        {
            Issuer = envDomainApi.hostname,
            Audience = envDomainOrdering.hostname,
            Subject = await GetSubject(memberKey),
            Claims = await GetClaims(memberKey),
            Expires = DateTime.UtcNow.AddMinutes(300),
            SigningCredentials = envDomainOrdering.GetSigningCredentials(),
        };
    }

    public async Task<string> GenerateToken(BillMemberKey memberKey)
    {
        var handler = new JsonWebTokenHandler();
        var tokenDescriptor = await GetTokenDescriptor(memberKey);
        var token = handler.CreateToken(tokenDescriptor);

        return token;
    }
}