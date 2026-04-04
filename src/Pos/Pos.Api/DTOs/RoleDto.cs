namespace FoodSphere.Pos.Api.DTO;

public record RoleRequest
{
    public required string name { get; set; }
    public string? description { get; set; }
    public ICollection<int> permission_ids { get; set; } = [];
}

public record RoleUpdateRequest
{
    public required string name { get; set; }
    public string? description { get; set; }
}

public record RoleResponse
{
    public required short id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime? update_time { get; set; }

    public required Guid restaurant_id { get; set; }

    public required string name { get; set; }
    public string? description { get; set; }
    public ICollection<int> permission_ids { get; set; } = [];

    public static readonly Expression<Func<Role, RoleResponse>> Projection =
        model => new()
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            restaurant_id = model.RestaurantId,
            name = model.Name,
            description = model.Description,
            permission_ids = model.Permissions
                .Select(rp => rp.PermissionId)
                .ToArray()
        };

    public static readonly Func<Role, RoleResponse> Project = Projection.Compile();
}