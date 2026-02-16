namespace FoodSphere.Pos.Api.Controller;

[Route("auth/staff")]
public class StaffAuthController(
    ILogger<StaffAuthController> logger,
    StaffPortalService staffPortalService
) : FoodSphereControllerBase
{
    /// <summary>
    /// login staff user
    /// </summary>
    [HttpPost("token")]
    public async Task<ActionResult<StaffTokenResponse>> GenerateToken(StaffTokenRequest body)
    {
        var portal = await staffPortalService.GetPortal(body.portal_id);

        if (portal is null)
        {
            return NotFound();
        }

        if (!portal.IsValid())
        {
            return BadRequest("staff portal is not valid.");
        }

        var token = await staffPortalService.GenerateToken(portal);

        await staffPortalService.SaveChanges();

        return new StaffTokenResponse
        {
            access_token = token
        };
    }
}