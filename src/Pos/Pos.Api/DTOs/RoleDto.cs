namespace FoodSphere.Pos.Api.DTOs;

public class RoleRequest
{
    public required string name { get; set; }
    public string? description { get; set; }
    public int[] permission_ids { get; set; } = [];

    public static RoleRequest FromModel(Role role)
    {
        return new RoleRequest
        {
            name = role.Name,
            description = role.Description,
            permission_ids = [.. role.Permissions.Select(rp => rp.PermissionId)]
        };
    }
}

public class RoleResponse
{
    public Guid restaurant_id { get; set; }
    public short id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public required string name { get; set; }
    public string? description { get; set; }
    public PermissionResponse[] permissions { get; set; } = [];

    public static RoleResponse FromModel(Role model)
    {
        return new RoleResponse
        {
            restaurant_id = model.RestaurantId,
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            name = model.Name,
            description = model.Description,
            permissions = [.. model.Permissions.Select(rp => PermissionResponse.FromModel(rp.Permission))]
        };
    }
}