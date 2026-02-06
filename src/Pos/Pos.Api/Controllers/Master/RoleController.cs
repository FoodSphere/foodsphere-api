namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}/roles")]
public class RoleController(
    ILogger<RoleController> logger,
    CheckPermissionService checkPermissionService,
    RoleService roleService
) : MasterControllerBase
{
    [HttpPost]
    public async Task<ActionResult<RoleResponse>> CreateRole(Guid restaurant_id, RoleRequest body)
    {
        var hasPermission = await checkPermissionService.CheckPermission(
            User, restaurant_id,
            PERMISSION.Role.CREATE
        );

        if (!hasPermission)
        {
            return Forbid();
        }

        var role = await roleService.CreateRole(
            restaurantId: restaurant_id,
            name: body.name,
            description: body.description
        );

        await roleService.SetPermissionsAsync(restaurant_id, role.Id, body.permission_ids);

        await roleService.SaveAsync();

        var populatedRole = await roleService.GetRole(restaurant_id, role.Id);

        return CreatedAtAction(
            nameof(GetRole),
            new { restaurant_id, role_id = role.Id },
            RoleResponse.FromModel(populatedRole!)
        );
    }

    [HttpGet]
    public async Task<ActionResult<List<RoleResponse>>> ListRoles(Guid restaurant_id)
    {
        var roles = await roleService.ListRoles(restaurant_id);

        return roles
            .Select(RoleResponse.FromModel)
            .ToList();
    }

    [HttpGet("{role_id}")]
    public async Task<ActionResult<RoleResponse>> GetRole(Guid restaurant_id, short role_id)
    {
        var role = await roleService.GetRole(restaurant_id, role_id);

        if (role is null)
        {
            return NotFound();
        }

        return RoleResponse.FromModel(role);
    }

    [HttpDelete("{role_id}")]
    public async Task<ActionResult> DeleteRole(Guid restaurant_id, short role_id)
    {
        var result = await roleService.DeleteRole(restaurant_id, role_id);

        if (result == false)
        {
            return NotFound();
        }
        else
        {
            await roleService.SaveAsync();
        }

        return NoContent();
    }

    [HttpPut("{role_id}/permissions")]
    public async Task<ActionResult> SetPermissions(Guid restaurant_id, short role_id, PermissionRequest body)
    {
        await roleService.SetPermissionsAsync(restaurant_id, role_id, body.permission_ids);

        await roleService.SaveAsync();

        return NoContent();
    }
}