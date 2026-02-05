using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace FoodSphere.Pos.Api.Controllers;

[PosAuthorize]
public abstract class PosControllerBase : FoodSphereControllerBase
{
    protected UserType UserType
    {
        get
        {
            var obj = HttpContext.Items[nameof(Common.Enums.UserType)];

            if (obj is not UserType userType)
            {
                throw new InvalidOperationException();
            }

            return userType;
        }
    }

    protected MasterUser MasterUser
    {
        get
        {
            var obj = HttpContext.Items[nameof(Common.Entities.MasterUser)];

            if (obj is not MasterUser user)
            {
                throw new InvalidOperationException();
            }

            return user;
        }
    }

    protected StaffUser StaffUser
    {
        get
        {
            var obj = HttpContext.Items[nameof(Common.Entities.StaffUser)];

            if (obj is not StaffUser user)
            {
                throw new InvalidOperationException();
            }

            return user;
        }
    }
}

[MasterAuthorize]
public abstract class MasterControllerBase : FoodSphereControllerBase
{
    protected string MasterId
    {
        get
        {
            var userId = User.FindFirstValue(FoodSphereClaimType.Identity.UserIdClaimType)
                ?? throw new InvalidOperationException();

            return userId;
        }
    }

    protected MasterUser MasterUser
    {
        get
        {
            var obj = HttpContext.Items[nameof(Common.Entities.MasterUser)];

            if (obj is not MasterUser user)
            {
                throw new InvalidOperationException();
            }

            return user;
        }
    }
}

[Route("current")]
public class CurrentController : FoodSphereControllerBase
{
    [PosAuthorize]
    [HttpGet("claims")]
    public ActionResult<string> GetClaims()
    {
        return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
    }
}