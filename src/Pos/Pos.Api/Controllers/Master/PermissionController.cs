namespace FoodSphere.Pos.Api.Controller;

[Route("permissions")]
public class PermissionController(
    ILogger<PermissionController> logger,
    PermissionService permissionService
) : MasterControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<PermissionResponse>>> ListPermissions()
    {
        var permissions = await permissionService.ListPermissions();

        return permissions
            .Select(PermissionResponse.FromModel)
            .ToList();
    }
}