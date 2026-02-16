namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}/roles")]
public class RoleController(
    ILogger<RoleController> logger,
    AccessControlService accessControlService,
    RoleService roleService
) : MasterControllerBase
{
    /// <summary>
    /// list roles
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ICollection<RoleResponse>>> ListRoles(Guid restaurant_id)
    {
        var responses = await roleService.QueryRoles()
            .Where(e =>
                e.RestaurantId == restaurant_id)
            .Select(RoleResponse.Projection)
            .ToArrayAsync();

        return responses;
    }

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

        var response = await roleService.GetRole(
            restaurant_id, role.Id,
            RoleResponse.Projection);

        return CreatedAtAction(
            nameof(GetRole),
            new { restaurant_id, role_id = role.Id },
            response);
    }

    /// <summary>
    /// get role
    /// </summary>
    [HttpGet("{role_id}")]
    public async Task<ActionResult<RoleResponse>> GetRole(Guid restaurant_id, short role_id)
    {
        var response = await roleService.GetRole(restaurant_id, role_id, RoleResponse.Projection);

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }

    /// <summary>
    /// delete role
    /// </summary>
    [HttpDelete("{role_id}")]
    public async Task<ActionResult> DeleteRole(Guid restaurant_id, short role_id)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// set role's permissions
    /// </summary>
    [HttpPut("{role_id}/permissions")]
    public async Task<ActionResult> SetPermissions(Guid restaurant_id, short role_id, int[] permission_ids)
    {
        await roleService.SetPermissions(restaurant_id, role_id, permission_ids);

        await roleService.SaveChanges();

        return NoContent();
    }
}