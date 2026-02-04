namespace FoodSphere.Pos.Api.Controllers;

[Route("auth/staff")]
public class StaffAuthController(
    ILogger<StaffAuthController> logger,
    StaffPortalService staffPortalService
) : FoodSphereControllerBase
{
    [HttpPost("token")]
    public async Task<ActionResult<StaffTokenResponse>> GenerateToken(StaffTokenRequest body)
    {
        var portal = await staffPortalService.GetStaffPortal(body.portal_id);

        if (portal is null)
        {
            return NotFound();
        }

        if (!portal.IsValid())
        {
            return BadRequest("staff portal is not valid.");
        }

        var token = await staffPortalService.GenerateToken(portal);

        await staffPortalService.SaveAsync();

        return new StaffTokenResponse
        {
            access_token = token
        };
    }
}