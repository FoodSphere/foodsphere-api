using Microsoft.AspNetCore.Mvc;
using FoodSphere.Services;
using FoodSphere.Data.Models;

namespace FoodSphere.Controllers.Auth;

public class StaffTokenResponse
{
    public required string access_token { get; set; }
}

[Route("auth/staff")]
public class StaffAuthController(
    ILogger<StaffAuthController> logger,
    StaffService staffService
) : AppController
{
    readonly ILogger<StaffAuthController> _logger = logger;
    readonly StaffService _staffService = staffService;

    [HttpPost("token")]
    public async Task<ActionResult<StaffTokenResponse>> GenerateToken([FromQuery] Guid portal_id)
    {
        var portal = await _staffService.GetStaffPortal(portal_id);

        if (portal is null)
        {
            return NotFound();
        }

        if (!portal.IsValid())
        {
            return BadRequest("staff portal is not valid.");
        }

        var token = await _staffService.GenerateToken(portal);

        await _staffService.Save();

        return new StaffTokenResponse
        {
            access_token = token
        };
    }
}