namespace FoodSphere.SelfOrdering.Api.Controllers;

[Route("auth")]
public class SelfOrderingAuthController(
    ILogger<SelfOrderingAuthController> logger,
    OrderingPortalService orderingPortalService
) : FoodSphereControllerBase
{
    [HttpPost("token")]
    public async Task<ActionResult<SelfOrderingTokenResponse>> GetToken(SelfOrderingTokenRequest body)
    {
        var portal = await orderingPortalService.GetPortal(body.portal_id);

        if (portal is null)
        {
            return NotFound();
        }

        if (!portal.IsValid())

        {
            return BadRequest("ordering portal is not valid.");
        }

        var token = await orderingPortalService.GenerateToken(portal);

        await orderingPortalService.SaveAsync();

        return new SelfOrderingTokenResponse
        {
            access_token = token
        };
    }
}