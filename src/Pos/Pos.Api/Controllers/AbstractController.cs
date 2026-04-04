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
                throw new InvalidOperationException();

            return userType;
        }
    }

    protected MasterUserKey MasterKey
    {
        get
        {
            var obj = HttpContext.Items[nameof(Common.Entity.MasterUserKey)];

            if (obj is not MasterUserKey userKey)
                throw new InvalidOperationException();

            return userKey;
        }
    }

    protected WorkerUserKey WorkerKey
    {
        get
        {
            var obj = HttpContext.Items[nameof(Common.Entity.WorkerUserKey)];

            if (obj is not WorkerUserKey userKey)
                throw new InvalidOperationException();

            return userKey;
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

    protected MasterUser MasterKey
    {
        get
        {
            var obj = HttpContext.Items[nameof(Common.Entity.MasterUserKey)];

            if (obj is not MasterUser user)
                throw new InvalidOperationException();

            return user;
        }
    }
}

[Route("current")]
public class CurrentController : FoodSphereControllerBase
{
    /// <summary>
    /// inspect claims in token
    /// </summary>
    [PosAuthorize]
    [HttpGet("claims")]
    public async Task<ActionResult<string>> GetClaims()
    {
        return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
    }
}