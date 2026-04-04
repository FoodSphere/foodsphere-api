namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}/roles")]
public class RoleController(
    ILogger<RoleController> logger,
    AccessControlService accessControl,
    RoleServiceBase roleService
) : MasterControllerBase
{
    /// <summary>
    /// list roles
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ICollection<RoleResponse>>> ListRoles(
        Guid restaurant_id)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Role.READ);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        Expression<Func<Role, bool>> predicate = e =>
            e.RestaurantId == restaurant_id;

        return await roleService.ListRoles(
            RoleResponse.Projection, predicate);
    }

    /// <summary>
    /// create role
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<RoleResponse>> CreateRole(
        Guid restaurant_id, RoleRequest body)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Role.CREATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var permissionKeys = body.permission_ids.Select(
            id => new PermissionKey(id));

        var roleResult = await roleService.CreateRole(
            RoleResponse.Projection, new(
                 new(restaurant_id),
                 body.name,
                 permissionKeys,
                 body.description));

        if (roleResult.IsFailed)
            return roleResult.Errors.ToActionResult();

        var (roleKey, response) = roleResult.Value;

        return CreatedAtAction(
            nameof(GetRole),
            new { restaurant_id, role_id = roleKey.Id },
            response);
    }

    /// <summary>
    /// get role
    /// </summary>
    [HttpGet("{role_id}")]
    public async Task<ActionResult<RoleResponse>> GetRole(
        Guid restaurant_id, short role_id)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Role.READ);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var response = await roleService.GetRole(
            RoleResponse.Projection,
            new(restaurant_id, role_id));

        if (response is null)
            return NotFound();

        return response;
    }

    /// <summary>
    /// update role
    /// </summary>
    [HttpPut("{role_id}")]
    public async Task<ActionResult> UpdateRole(
        Guid restaurant_id, short role_id, RoleUpdateRequest body)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Role.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var result = await roleService.UpdateRole(
            new(restaurant_id, role_id), new(
                body.name,
                body.description));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }

    /// <summary>
    /// delete role
    /// </summary>
    [HttpDelete("{role_id}")]
    public async Task<ActionResult> DeleteRole(Guid restaurant_id, short role_id)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Role.DELETE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var result = await roleService.DeleteRole(
            new(restaurant_id, role_id));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }

    /// <summary>
    /// set role's permissions
    /// </summary>
    [HttpPut("{role_id}/permissions")]
    public async Task<ActionResult> SetPermissions(Guid restaurant_id, short role_id, int[] permission_ids)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Role.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var permissionKeys = permission_ids.Select(
            id => new PermissionKey(id));

        var result = await roleService.SetPermissions(
            new(restaurant_id, role_id),
            permissionKeys);

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }
}