using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace FoodSphere.Consumer.Api.Controllers;

[ConsumerAuthorize]
public abstract class ConsumerControllerBase : FoodSphereControllerBase
{
    protected Guid ConsumerId
    {
        get
        {
            var userId = User.FindFirstValue(FoodSphereClaimType.Identity.UserIdClaimType)
                ?? throw new InvalidOperationException();

            return Guid.Parse(userId);
        }
    }
}

[Route("current")]
public class CurrentController : FoodSphereControllerBase
{
    [ConsumerAuthorize]
    [HttpGet("claims")]
    public ActionResult<string> GetClaims()
    {
        return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
    }
}