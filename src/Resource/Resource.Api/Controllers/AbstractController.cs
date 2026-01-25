using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace FoodSphere.Resource.Api.Controllers;

[ResourceAuthorize]
public abstract class ResourceControllerBase : FoodSphereControllerBase
{
    protected string UserId
    {
        get
        {
            var userId = User.FindFirstValue(FoodSphereClaimType.Identity.UserIdClaimType)
                ?? throw new InvalidOperationException();

            return userId;
        }
    }

    protected string UserRole
    {
        get
        {
            var role = User.FindFirstValue(FoodSphereClaimType.Identity.RoleClaimType)
                ?? throw new InvalidOperationException();

            return role;
        }
    }
}

[Route("current")]
public class CurrentController : FoodSphereControllerBase
{
    [ResourceAuthorize]
    [HttpGet("claims")]
    public ActionResult<string> GetClaims()
    {
        return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
    }
}