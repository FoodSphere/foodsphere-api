namespace FoodSphere.Resource.Api.DTO;

public class BillRequest
{
    public Guid restaurant_id { get; set; }
    public short branch_id { get; set; }
    public short table_id { get; set; }

    public Guid? consumer_id { get; set; }
    public short? pax { get; set; }
}

public class BillResponse
{
    public Guid id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid? consumer_id { get; set; }
    public Guid restaurant_id { get; set; }
    public short branch_id { get; set; }
    public short table_id { get; set; }

    public IReadOnlyCollection<OrderResponse> orders { get; set; } = [];

    public short? pax { get; set; }
    public BillStatus status { get; set; }

    public static readonly Func<Bill, BillResponse> Project = Projection.Compile();

    public static Expression<Func<Bill, BillResponse>> Projection =>
        model => new BillResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            consumer_id = model.ConsumerId,
            restaurant_id = model.RestaurantId,
            branch_id = model.BranchId,
            table_id = model.TableId,
            orders = model.Orders
                .Select(e => OrderResponse.Projection.Invoke(e))
                .ToArray(),
            pax = model.Pax,
            status = model.Status,
        };
}