namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}/roles")]
public class RoleController(
    ILogger<RoleController> logger,
    AccessControlService accessControlService,
    RoleService roleService
) : MasterControllerBase
{
    /// <summary>
    /// create role
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<RoleResponse>> CreateRole(Guid restaurant_id, RoleRequest body)
    {
        var hasPermission = await accessControlService.Validate(
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

        await roleService.SetPermissions(restaurant_id, role.Id, body.permission_ids);

        await roleService.SaveChanges();

        var populatedRole = await roleService.GetRole(restaurant_id, role.Id);

        return CreatedAtAction(
            nameof(GetRole),
            new { restaurant_id, role_id = role.Id },
            RoleResponse.FromModel(populatedRole!)
        );
    }

    /// <summary>
    /// list roles
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<RoleResponse>>> ListRoles(Guid restaurant_id)
    {
        var roles = await roleService.ListRoles(restaurant_id);

        return roles
            .Select(RoleResponse.FromModel)
            .ToList();
    }

    /// <summary>
    /// get role
    /// </summary>
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

    /// <summary>
    /// delete role
    /// </summary>
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
            await roleService.SaveChanges();
        }

        return NoContent();
    }

    /// <summary>
    /// set role's permissions
    /// </summary>
    [HttpPut("{role_id}/permissions")]
    public async Task<ActionResult> SetPermissions(Guid restaurant_id, short role_id, PermissionRequest body)
    {
        await roleService.SetPermissions(restaurant_id, role_id, body.permission_ids);

        await roleService.SaveChanges();

        return NoContent();
    }
}