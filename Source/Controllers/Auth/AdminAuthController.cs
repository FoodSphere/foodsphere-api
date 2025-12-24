using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using FoodSphere.Services;
using FoodSphere.Data.Models;

namespace FoodSphere.Controllers.Auth;

public class AdminTokenRequest
{
    public required string identifier { get; set; }
}

public class AdminTokenResponse
{
    public required string access_token { get; set; }
}

[Route("auth/admin")]
public class AdminAuthController(
    ILogger<AdminAuthController> logger,
    AdminAuthService authService
) : AppController
{
    readonly ILogger<AdminAuthController> _logger = logger;
    readonly AdminAuthService _authService = authService;

    [HttpPost("token")]
    public async Task<ActionResult<AdminTokenResponse>> PostAdminToken(AdminTokenRequest body)
    {
        var token = await _authService.GenerateToken(body.identifier);

        var response = new AdminTokenResponse
        {
            access_token = token,
        };

        return response;
    }
}