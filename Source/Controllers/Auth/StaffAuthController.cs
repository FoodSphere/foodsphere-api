using Microsoft.AspNetCore.Mvc;
using FoodSphere.Services;
using FoodSphere.Data.Models;

namespace FoodSphere.Controllers.Auth;

[Route("auth/staff")]
public class StaffAuthController(
    ILogger<StaffAuthController> logger,
    BranchService branchService
) : AppController
{
    readonly ILogger<StaffAuthController> _logger = logger;
    readonly BranchService _branchService = branchService;

    [HttpPost("token")]
    public async Task<ActionResult<string>> GenerateToken([FromQuery] Guid portal_id)
    {
        var portal = await _branchService.GetStaffPortal(portal_id);

        if (portal is null)
        {
            return NotFound();
        }

        return "";
    }
}