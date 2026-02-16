namespace FoodSphere.Pos.Api.DTO;

public class RoleRequest
{
    public required string name { get; set; }
    public string? description { get; set; }
    public IReadOnlyCollection<int> permission_ids { get; set; } = [];
}

public class RoleResponse
{
    public short id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid restaurant_id { get; set; }

    public required string name { get; set; }
    public string? description { get; set; }
    public IReadOnlyCollection<int> permission_ids { get; set; } = [];

    public static readonly Func<Role, RoleResponse> Project = Projection.Compile();

    public static Expression<Func<Role, RoleResponse>> Projection =>
        model => new RoleResponse
        {
            restaurant_id = model.RestaurantId,
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            name = model.Name,
            description = model.Description,
            permission_ids = model.Permissions
                .Select(rp => rp.PermissionId)
                .ToArray()
        };
}