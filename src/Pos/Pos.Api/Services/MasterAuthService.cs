using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace FoodSphere.Pos.Api.Service;

public class MasterAuthService(
    IOptions<EnvDomainApi> envDomainApi,
    IOptions<EnvDomainMaster> envDomainMaster,
    IOptions<EnvDomainPos> envDomainPos,
    UserManager<MasterUser> userManager,
    AuthorizeHelperService authService)
{
    readonly EnvDomainApi envDomainApi = envDomainApi.Value;
    readonly EnvDomainMaster envDomainMaster = envDomainMaster.Value;
    readonly EnvDomainPos envDomainPos = envDomainPos.Value;

    async Task<ClaimsIdentity> GetSubject(MasterUser user)
    {
        List<Claim> claims = [
            new(FoodSphereClaimType.Identity.UserIdClaimType, user.Id),
            new(FoodSphereClaimType.Identity.SecurityStampClaimType, user.SecurityStamp ?? string.Empty),
        ];

        return new(claims);
    }

    async Task<Dictionary<string, object>> GetClaims(MasterUser user)
    {
        return new()
        {
            [FoodSphereClaimType.UserTypeClaimType] =  UserType.Master.ToString(),
        };
    }

    async Task<SecurityTokenDescriptor> GetTokenDescriptor(MasterUser user)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = envDomainApi.hostname,
            Subject = await GetSubject(user),
            Claims = await GetClaims(user),
            Expires = DateTime.UtcNow.AddMinutes(300),
            SigningCredentials = envDomainMaster.GetSigningCredentials(),
        };

        tokenDescriptor.Audiences.Add(envDomainMaster.hostname);
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

    public async Task<bool> IsTwoFactorEnabled(MasterUser user)
    {
        return
            await userManager.GetTwoFactorEnabledAsync(user) &&
            (await userManager.GetValidTwoFactorProvidersAsync(user)).Count > 0;
    }
}