namespace FoodSphere.SelfOrdering.Api.Controller;

[Route("auth")]
public class SelfOrderingAuthController(
    ILogger<SelfOrderingAuthController> logger,
    OrderingPortalServiceBase portalService
) : FoodSphereControllerBase
{
    [HttpPost("token")]
    public async Task<ActionResult<SelfOrderingTokenResponse>> GetToken(
        SelfOrderingTokenRequest body)
    {
        var result = await portalService.GenerateToken(
            new(body.portal_id), null, null);

        if (!result.TryGetValue(out var token))
            return result.Errors.ToActionResult();

        return new SelfOrderingTokenResponse
        {
            access_token = token
        };
    }
}