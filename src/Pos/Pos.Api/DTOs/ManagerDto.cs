namespace FoodSphere.Pos.Api.DTO;

public class ManagerRequest
{
    public required string master_id;

    public short[] roles = [];
}

public class BranchManagerResponse
{
    public required string master_id;
    public Guid restaurant_id;
    public short branch_id;

    public DateTime create_time;
    public DateTime update_time;

    public int[] permissions = [];

    public static BranchManagerResponse FromModel(BranchManager model)
    {
        return new BranchManagerResponse
        {
            master_id = model.MasterId,
            restaurant_id = model.RestaurantId,
            branch_id = model.BranchId,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            permissions = [.. model.GetPermissions().Select(p => p.Id)],
        };
    }
}

public class RestaurantManagerResponse
{
    public required string master_id;
    public Guid restaurant_id;

    public DateTime create_time;
    public DateTime update_time;

    public int[] permissions = [];

    public static RestaurantManagerResponse FromModel(RestaurantManager model)
    {
        return new RestaurantManagerResponse
        {
            master_id = model.MasterId,
            restaurant_id = model.RestaurantId,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            permissions = [.. model.GetPermissions().Select(p => p.Id)],
        };
    }
}