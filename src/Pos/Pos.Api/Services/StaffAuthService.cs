using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace FoodSphere.Pos.Api.Services;

public class StaffAuthService(
    IOptions<EnvDomainApi> envDomainApi,
    IOptions<EnvDomainPos> envDomainPos
) {
    readonly EnvDomainApi envDomainApi = envDomainApi.Value;
    readonly EnvDomainPos envDomainPos = envDomainPos.Value;

    async Task<ClaimsIdentity> GetSubject(StaffUser user)
    {
        List<Claim> claims = [
            new(FoodSphereClaimType.Identity.UserIdClaimType, user.Id.ToString()),
        ];

        return new ClaimsIdentity(claims);
    }

    async Task<Dictionary<string, object>> GetClaims(StaffUser user)
    {
        var claims = new Dictionary<string, object>
        {
            [FoodSphereClaimType.RestaurantClaimType] = user.RestaurantId,
            [FoodSphereClaimType.BranchClaimType] = user.BranchId,
            [FoodSphereClaimType.UserTypeClaimType] = UserType.Staff.ToString(),
        };

        return claims;
    }

    async Task<SecurityTokenDescriptor> GetTokenDescriptor(StaffUser user)
    {
        return new SecurityTokenDescriptor
        {
            Issuer = envDomainApi.hostname,
            Audience = envDomainPos.hostname,
            Subject = await GetSubject(user),
            Claims = await GetClaims(user),
            Expires = DateTime.UtcNow.AddMinutes(300),
            SigningCredentials = envDomainPos.GetSigningCredentials(),
        };
    }

    public async Task<string> GenerateToken(StaffUser user)
    {
        var handler = new JsonWebTokenHandler();
        var tokenDescriptor = await GetTokenDescriptor(user);
        var token = handler.CreateToken(tokenDescriptor);

        return token;
    }
}