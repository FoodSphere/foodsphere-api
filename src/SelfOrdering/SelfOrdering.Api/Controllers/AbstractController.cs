using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace FoodSphere.SelfOrdering.Api.Controllers;

[SelfOrderingAuthorize]
public abstract class SelfOrderingControllerBase : FoodSphereControllerBase
{
    protected BillMember Member
    {
        get
        {
            var obj = HttpContext.Items[nameof(Common.Entities.BillMember)];

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
    [SelfOrderingAuthorize]
    [HttpGet("claims")]
    public ActionResult<string> GetClaims()
    {
        return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
    }
}