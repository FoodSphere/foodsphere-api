using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using FoodSphere.Services;
using FoodSphere.Data.Models;

namespace FoodSphere.Controllers.Auth;

[Route("resource/users")]
public class ResourceUserController(
    ILogger<ResourceUserController> logger,
    UserManager<MasterUser> userManager
) : AppController
{
    readonly ILogger<ResourceUserController> _logger = logger;
    readonly UserManager<MasterUser> _userManager = userManager;

    [HttpGet("masters")]
    public ActionResult<List<MasterUser>> ListMasterUsers()
    {
        return _userManager.Users.ToList();
    }
}