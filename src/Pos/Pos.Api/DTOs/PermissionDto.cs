namespace FoodSphere.Pos.Api.DTO;

public record PermissionResponse
{
    public required int id { get; set; }
    public required string name { get; set; }
    public string? description { get; set; }

    public static readonly Expression<Func<Permission, PermissionResponse>> Projection =
        model => new()
        {
            id = model.Id,
            name = model.Name,
            description = model.Description
        };

    public static readonly Func<Permission, PermissionResponse> Project = Projection.Compile();
}

public record CurrentAuthResponse
{
    public required bool is_owner { get; set; }
    public ICollection<PermissionResponse> permissions { get; set; } = [];
}