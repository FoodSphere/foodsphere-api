using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FoodSphere.Services;
using FoodSphere.Data.Models;

namespace FoodSphere.Controllers.Auth;

public class SelfOrderingTokenResponse
{
    public required string access_token { get; set; }
}

[Route("auth/self-ordering")]
public class SelfOrderingAuthController(
    ILogger<SelfOrderingAuthController> logger,
    OrderingService orderingService
) : ConsumerController
{
    readonly ILogger<SelfOrderingAuthController> _logger = logger;
    readonly OrderingService _orderingService = orderingService;

    [AllowAnonymous]
    [HttpPost("token")]
    public async Task<ActionResult<SelfOrderingTokenResponse>> GetToken([FromQuery] Guid portal_id)
    {
        var portal = await _orderingService.GetPortal(portal_id);

        if (portal is null)
        {
            return NotFound();
        }

        if (!portal.IsValid())
        {
            return BadRequest("ordering portal is not valid.");
        }

        Guid? consumerId = null;

        if (User?.Identity?.IsAuthenticated is true)
        {
            consumerId = UserId;
        }

        _logger.LogInformation("authenticated User ID: {ConsumerId}", consumerId?.ToString() ?? "null");

        // var authResult = await HttpContext.AuthenticateAsync(JwtClientConfiguration.SchemeName);

        // if (authResult.Succeeded)
        // {
        //     consumerId = UserId;
        // }

        var token = await _orderingService.GenerateToken(portal, consumerId);

        await _orderingService.Save();

        return new SelfOrderingTokenResponse
        {
            access_token = token
        };
    }
}