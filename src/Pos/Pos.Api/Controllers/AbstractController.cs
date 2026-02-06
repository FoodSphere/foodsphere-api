using System.Security.Claims;

namespace FoodSphere.Pos.Api.Controller;

[PosAuthorize]
public abstract class PosControllerBase : FoodSphereControllerBase
{
    protected UserType UserType
    {
        get
        {
            var obj = HttpContext.Items[nameof(Common.Constant.UserType)];

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
            var obj = HttpContext.Items[nameof(Common.Entity.MasterUser)];

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
            var obj = HttpContext.Items[nameof(Common.Entity.StaffUser)];

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
            var obj = HttpContext.Items[nameof(Common.Entity.MasterUser)];

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
    public async Task<ActionResult<string>> GetClaims()
    {
        return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
    }
}