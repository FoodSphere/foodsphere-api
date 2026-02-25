namespace FoodSphere.Pos.Api.DTO;

public class PermissionRequest
{
    public int[] permission_ids { get; set; } = [];
}

public class PermissionResponse
{
    public int id { get; set; }
    public string name { get; set; } = null!;
    public string? description { get; set; }

    public static PermissionResponse FromModel(Permission model)
    {
        return new PermissionResponse
        {
            id = model.Id,
            name = model.Name,
            description = model.Description
        };
    }
}