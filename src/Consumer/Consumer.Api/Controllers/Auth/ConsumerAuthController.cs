namespace FoodSphere.Consumer.Api.Controllers;

[Route("auth")]
public class ConsumerAuthController(
    ILogger<ConsumerAuthController> logger,
    ConsumerAuthService authService
) : FoodSphereControllerBase
{
    // [HttpPost("token")]
    // public async Task<ActionResult<ConsumerTokenResponse>> GenerateToken(ConsumerTokenRequest body)
    // {
    //     // var token = await authService.GenerateToken();

    //     var response = new ConsumerTokenResponse
    //     {
    //         access_token = "token",
    //     };

    //     return response;
    // }
}
