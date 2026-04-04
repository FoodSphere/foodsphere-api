namespace FoodSphere.Pos.Api.Controller;

[Route("auth/worker")]
public class WorkerAuthController(
    ILogger<WorkerAuthController> logger,
    WorkerPortalService portalService
) : FoodSphereControllerBase
{
    /// <summary>
    /// login worker user
    /// </summary>
    [HttpPost("token")]
    public async Task<ActionResult<WorkerTokenResponse>> GenerateToken(
        WorkerTokenRequest body)
    {
        var result = await portalService.GenerateToken(
            new(body.portal_id));

        if (!result.TryGetValue(out var token))
            return result.Errors.ToActionResult();

        return new WorkerTokenResponse
        {
            access_token = token
        };
    }
}