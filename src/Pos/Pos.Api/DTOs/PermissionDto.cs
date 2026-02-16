namespace FoodSphere.Pos.Api.DTO;

public class PermissionResponse
{
    public int id { get; set; }
    public string name { get; set; } = null!;
    public string? description { get; set; }

    public static readonly Func<Permission, PermissionResponse> Project = Projection.Compile();

    public static Expression<Func<Permission, PermissionResponse>> Projection =>
        model => new PermissionResponse
        {
            id = model.Id,
            name = model.Name,
            description = model.Description
        };
}