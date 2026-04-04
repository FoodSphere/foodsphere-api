using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace FoodSphere.Pos.Api.Service;

public class WorkerAuthService(
    IOptions<EnvDomainApi> envDomainApi,
    IOptions<EnvDomainPos> envDomainPos,
    AuthorizeHelperService authService)
{
    readonly EnvDomainApi envDomainApi = envDomainApi.Value;
    readonly EnvDomainPos envDomainPos = envDomainPos.Value;

    async Task<ClaimsIdentity> GetSubject(WorkerUserKey user)
    {
        List<Claim> claims = [];

        // claims.AddRange(
        //     roles.Select(role =>
        //         new Claim(FoodSphereClaimType.Identity.RoleClaimType, role))
        // );

        return new(claims);
    }

    async Task<Dictionary<string, object>> GetClaims(WorkerUserKey user)
    {
        var roleMaps = await authService.ListWorkerRoles(
            user, r => new
            {
                Name = r.Name,
                PermissionIds = r.Permissions
                    .Select(p => p.PermissionId)
                    .ToArray()
            });

        return new()
        {
            [FoodSphereClaimType.Identity.UserIdClaimType] = (int)user.Id,
            [FoodSphereClaimType.Identity.RoleClaimType] = roleMaps
                .Select(r => r.Name)
                .ToArray(),
            [FoodSphereClaimType.RestaurantClaimType] = user.RestaurantId,
            [FoodSphereClaimType.BranchClaimType] = (int)user.BranchId,
            [FoodSphereClaimType.UserTypeClaimType] = UserType.Worker.ToString(),
            [FoodSphereClaimType.PermissionsClaimType] = roleMaps
                .SelectMany(r => r.PermissionIds)
                .Distinct()
                .ToArray()
        };
    }

    async Task<SecurityTokenDescriptor> GetTokenDescriptor(WorkerUserKey user)
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

    public async Task<string> GenerateToken(WorkerUserKey user)
    {
        var handler = new JsonWebTokenHandler();
        var tokenDescriptor = await GetTokenDescriptor(user);
        var token = handler.CreateToken(tokenDescriptor);

        return token;
    }
}