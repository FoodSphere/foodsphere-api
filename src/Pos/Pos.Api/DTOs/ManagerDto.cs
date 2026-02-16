namespace FoodSphere.Pos.Api.DTO;

public class ManagerRequest
{
    public required string master_id;

    public IReadOnlyCollection<short> roles = [];
}

public class RestaurantManagerRequest
{
    public IReadOnlyCollection<short> roles = [];
}

public class BranchManagerResponse
{
    public required string master_id;
    public Guid restaurant_id;
    public short branch_id;

    public DateTime create_time;
    public DateTime update_time;

    public IReadOnlyCollection<int> permissions = [];

    public static readonly Func<BranchManager, BranchManagerResponse> Project = Projection.Compile();

    public static Expression<Func<BranchManager, BranchManagerResponse>> Projection =>
        model => new BranchManagerResponse
        {
            master_id = model.MasterId,
            restaurant_id = model.RestaurantId,
            branch_id = model.BranchId,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            permissions = model.Roles
                .SelectMany(r => r.Role.Permissions)
                .Select(rp => rp.Permission)
                .Select(p => p.Id)
                .Distinct()
                .ToArray(),
        };
}

public class RestaurantManagerResponse
{
    public required string master_id;
    public Guid restaurant_id;

    public DateTime create_time;
    public DateTime update_time;

    public IReadOnlyCollection<int> permissions = [];

    public static readonly Func<RestaurantManager, RestaurantManagerResponse> Project = Projection.Compile();

    public static Expression<Func<RestaurantManager, RestaurantManagerResponse>> Projection =>
        model => new RestaurantManagerResponse
        {
            master_id = model.MasterId,
            restaurant_id = model.RestaurantId,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            permissions = model.Roles
                .SelectMany(r => r.Role.Permissions)
                .Select(rp => rp.Permission)
                .Select(p => p.Id)
                .Distinct()
                .ToArray(),
        };
}