using Microsoft.AspNetCore.Mvc;

namespace FoodSphere.Resource.Api.Controllers;

[Route("auth")]
public class ResourceAuthController(
    ILogger<ResourceAuthController> logger,
    ResourceAuthService authService
) : FoodSphereControllerBase
{
    [HttpPost("token")]
    public async Task<ActionResult<ResourceTokenResponse>> GenerateToken(ResourceTokenRequest body)
    {
        var token = await authService.GenerateToken(body.identifier);

        logger.LogInformation("Generated token for {identifier}", body.identifier);

        var response = new ResourceTokenResponse
        {
            access_token = token,
        };

        return response;
    }
}