using System.Security.Claims;

namespace FoodSphere.SelfOrdering.Api.Controller;

[SelfOrderingAuthorize]
public abstract class SelfOrderingControllerBase : FoodSphereControllerBase
{
    protected BillMember Member
    {
        get
        {
            var obj = HttpContext.Items[nameof(Common.Entity.BillMember)];

            if (obj is not BillMember member)
            {
                throw new InvalidOperationException();
            }

            return member;
        }
    }
}

[Route("current")]
public class CurrentController : FoodSphereControllerBase
{
    /// <summary>
    /// inspect claims in token
    /// </summary>
    [SelfOrderingAuthorize]
    [HttpGet("claims")]
    public ActionResult<string> GetClaims()
    {
        return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
    }
}