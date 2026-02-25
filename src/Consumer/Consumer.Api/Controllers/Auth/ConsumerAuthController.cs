namespace FoodSphere.Consumer.Api.Controller;

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
