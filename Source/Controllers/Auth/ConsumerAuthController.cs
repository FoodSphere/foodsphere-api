using Microsoft.AspNetCore.Mvc;
using FoodSphere.Services;

namespace FoodSphere.Controllers.Auth;

public class ConsumerTokenRequest
{
}

public class ConsumerTokenResponse
{
    public required string access_token { get; set; }
}

[Route("auth/consumer")]
public class ConsumerAuthController(
    ILogger<ConsumerAuthController> logger,
    ConsumerAuthService authService
) : AppController
{
    readonly ILogger<ConsumerAuthController> _logger = logger;
    readonly ConsumerAuthService _authService = authService;

    // [HttpPost("token")]
    // public async Task<ActionResult<ConsumerTokenResponse>> PostConsumerToken(ConsumerTokenRequest body)
    // {
    //     var token = await _authService.GenerateToken();

    //     var response = new ConsumerTokenResponse
    //     {
    //         access_token = token,
    //     };

    //     return response;
    // }
}
