using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace FoodSphere.Pos.Api.Services;

public class MasterAuthService(
    UserManager<MasterUser> userManager,
    IOptions<EnvDomainApi> envDomainApi,
    IOptions<EnvDomainMaster> envDomainMaster,
    IOptions<EnvDomainPos> envDomainPos
) {
    readonly EnvDomainApi envDomainApi = envDomainApi.Value;
    readonly EnvDomainMaster envDomainMaster = envDomainMaster.Value;
    readonly EnvDomainPos envDomainPos = envDomainPos.Value;

    async Task<ClaimsIdentity> GetSubject(MasterUser user)
    {
        List<Claim> claims = [
            new(FoodSphereClaimType.Identity.UserIdClaimType, user.Id),
            new(FoodSphereClaimType.Identity.SecurityStampClaimType, user.SecurityStamp ?? string.Empty),
        ];

        var roles = await userManager.GetRolesAsync(user);

        claims.AddRange(
            roles.Select(role => new Claim(FoodSphereClaimType.Identity.RoleClaimType, role))
        );

        return new ClaimsIdentity(claims);
    }

    async Task<Dictionary<string, object>> GetClaims(MasterUser user)
    {
        var claims = new Dictionary<string, object>
        {
            [FoodSphereClaimType.UserTypeClaimType] =  UserType.Master.ToString(),
        };

        return claims;
    }

    async Task<SecurityTokenDescriptor> GetTokenDescriptor(MasterUser user)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = envDomainApi.hostname,
            Audience = envDomainMaster.hostname,
            Subject = await GetSubject(user),
            Claims = await GetClaims(user),
            Expires = DateTime.UtcNow.AddMinutes(300),
            SigningCredentials = envDomainMaster.GetSigningCredentials(),
        };

        // tokenDescriptor.Audiences.Add(envDomainMaster.hostname);
        tokenDescriptor.Audiences.Add(envDomainPos.hostname);

        return tokenDescriptor;
    }

    public async Task<string> GenerateToken(MasterUser user)
    {
        var handler = new JsonWebTokenHandler();
        var tokenDescriptor = await GetTokenDescriptor(user);
        var token = handler.CreateToken(tokenDescriptor);

        return token;
    }

    public async Task<bool> IsTwoFactorEnabledAsync(MasterUser user)
    {
        return
            await userManager.GetTwoFactorEnabledAsync(user) &&
            (await userManager.GetValidTwoFactorProvidersAsync(user)).Count > 0;
    }
}