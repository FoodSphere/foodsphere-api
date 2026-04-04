using System.Security.Claims;

namespace FoodSphere.SelfOrdering.Api.Controller;

[OrderingAuthorize]
public abstract class SelfOrderingControllerBase : FoodSphereControllerBase
{
    protected BillMemberKey MemberKey
    {
        get
        {
            var obj = HttpContext.Items[nameof(BillMemberKey)];

            if (obj is not BillMemberKey key)
                throw new InvalidOperationException();

            return key;
        }
    }

    protected BranchKey BranchKey
    {
        get
        {
            var obj = HttpContext.Items[nameof(Common.Entity.BranchKey)];

            if (obj is not BranchKey key)
                throw new InvalidOperationException();

            return key;
        }
    }

    protected Guid RestaurantId => BranchKey.RestaurantId;
}

[Route("current")]
public class CurrentController : FoodSphereControllerBase
{
    /// <summary>
    /// inspect claims in token
    /// </summary>
    [OrderingAuthorize]
    [HttpGet("claims")]
    public ActionResult<string> GetClaims()
    {
        return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
    }
}