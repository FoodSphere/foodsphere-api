namespace FoodSphere.Pos.Api.DTO;

public class BranchManagerRequest
{
    public required string master_id;
}

public class BranchManagerResponse
{
    public Guid restaurant_id;

    public DateTime create_time;
    public DateTime update_time;

    public short branch_id;
    public required string master_id;

    public static BranchManagerResponse FromModel(BranchManager model)
    {
        return new BranchManagerResponse
        {
            restaurant_id = model.RestaurantId,
            branch_id = model.BranchId,
            master_id = model.MasterId,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
        };
    }
}