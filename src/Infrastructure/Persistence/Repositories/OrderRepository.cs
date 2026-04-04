namespace FoodSphere.Infrastructure.Repository;

public class OrderRepository(
    FoodSphereDbContext context,
    IPublishEndpoint publishEndpoint
) : RepositoryBase(context)
{
    public async Task<ResultObject<Order>> CreateOrder(
        BillKey billKey,
        CancellationToken ct = default)
    {
        var bill = await _ctx.FindAsync<Bill>(billKey, ct);

        if (bill is null)
            return ResultObject.NotFound(billKey);

        if (bill.Status is not BillStatus.Open)
            return ResultObject.Fail(ResultError.State,
                "Only orders for open bills can be created.");

        int lastId;
        var hasPendingAdds = _ctx.ChangeTracker.Entries<Order>()
            .Any(e =>
                e.State == EntityState.Added &&
                e.Entity.BillId == bill.Id);

        if (hasPendingAdds)
        {
            lastId = _ctx.Set<Order>().Local
                .Where(e => e.BillId == bill.Id)
                .Max(e => (int?)e.Id) ?? 0;
        }
        else
        {
            lastId = await _ctx.Set<Order>()
                .Where(e => e.BillId == bill.Id)
                .MaxAsync(e => (int?)e.Id, ct) ?? 0;
        }

        var order = new Order
        {
            Id = (short)(lastId + 1),
            BillId = bill.Id,
        };

        _ctx.Add(order);

        await publishEndpoint.Publish(
            new OrderCreatedMessage
            {
                Resource = new(order.BillId, order.Id),
                Branch = new(bill.RestaurantId, bill.BranchId)
            },
            ct);

        return order;
    }

    public IQueryable<Order> QueryOrders()
    {
        return _ctx.Set<Order>()
            .AsExpandable();
    }

    public IQueryable<Order> QueryOrders(BillKey billKey)
    {
        return QueryOrders()
            .Where(e =>
                e.BillId == billKey.Id);
    }

    public IQueryable<Order> QuerySingleOrder(OrderKey key)
    {
        return QueryOrders()
            .Where(e =>
                e.BillId == key.BillId &&
                e.Id == key.Id);
    }

    public async Task<Order?> GetOrder(OrderKey key, CancellationToken ct = default)
    {
        return await _ctx.FindAsync<Order>(key, ct);
    }

    public async Task<ResultObject> UpdateOrderStatus(
        OrderKey key,
        OrderStatus status,
        CancellationToken ct = default)
    {
        var order = await GetOrder(key, ct);

        if (order is null)
            return ResultObject.NotFound(key);

        if (order.Status == status)
            return ResultObject.Success();;

        if (order.Status is OrderStatus.Cancelled ||
            order.Status is OrderStatus.Served)
            return ResultObject.Fail(ResultError.State, "Invalid order status transition.");

        order.Status = status;

        await publishEndpoint.Publish(
            new OrderStatusUpdatedMessage
            {
                Resource = new(order.BillId, order.Id),
                Status = status,
            },
            ct);

        return ResultObject.Success();
    }

    public async Task<ResultObject> DeleteOrder(OrderKey key, CancellationToken ct = default)
    {
        var order = await GetOrder(key, ct);

        if (order is null)
            return ResultObject.NotFound(key);

        if (order.Status is not OrderStatus.Pending and not OrderStatus.Draft)
            return ResultObject.Fail(ResultError.State, "Only draft and pending orders can be deleted.");

        _ctx.Remove(order);

        return ResultObject.Success();
    }

    public async Task<ResultObject<OrderItem>> CreateItem(
        OrderKey orderKey,
        MenuKey menuKey,
        short quantity,
        string? note,
        CancellationToken ct = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);

        var order = await GetOrder(orderKey, ct);

        if (order is null)
            return ResultObject.NotFound(orderKey);

        var billKey = new BillKey(orderKey.BillId);
        var bill = await _ctx.FindAsync<Bill>(billKey, ct);

        if (bill is null)
            return ResultObject.NotFound(billKey);

        if (bill.RestaurantId != menuKey.RestaurantId)
            return ResultObject.Fail(ResultError.NotFound, "menu not match");

        if (bill.Status is not BillStatus.Open)
            return ResultObject.Fail(ResultError.State,
                "Only order item for open bills can be created.");

        var menu = await _ctx.FindAsync<Menu>(menuKey, ct);

        if (menu is null)
            return ResultObject.NotFound(menuKey);

        int lastId;
        var hasPendingAdds = _ctx.ChangeTracker.Entries<OrderItem>()
            .Any(e =>
                e.State == EntityState.Added &&
                e.Entity.BillId == orderKey.BillId &&
                e.Entity.OrderId == orderKey.Id);

        if (hasPendingAdds)
        {
            lastId = _ctx.Set<OrderItem>().Local
                .Where(e =>
                    e.BillId == orderKey.BillId &&
                    e.OrderId == orderKey.Id)
                .Max(e => (int?)e.Id) ?? 0;
        }
        else
        {
            lastId = await _ctx.Set<OrderItem>()
                .Where(e =>
                    e.BillId == orderKey.BillId &&
                    e.OrderId == orderKey.Id)
                .MaxAsync(e => (int?)e.Id, ct) ?? 0;
        }

        var item = new OrderItem
        {
            Id = (short)(lastId + 1),
            BillId = orderKey.BillId,
            OrderId = orderKey.Id,
            RestaurantId = menuKey.RestaurantId,
            MenuId = menuKey.Id,
            NameSnapshot = menu.Name,
            PriceSnapshot = menu.Price,
            Quantity = quantity,
            Note = note,
        };

        _ctx.Add(item);

        return item;
    }

    public IQueryable<OrderItem> QueryItems()
    {
        return _ctx.Set<OrderItem>()
            .AsExpandable();
    }

    public IQueryable<OrderItem> QueryItems(OrderKey key)
    {
        return QueryItems()
            .Where(e =>
                e.BillId == key.BillId &&
                e.OrderId == key.Id);
    }

    public IQueryable<OrderItem> QuerySingleItem(OrderItemKey key)
    {
        return QueryItems()
            .Where(e =>
                e.BillId == key.BillId &&
                e.OrderId == key.OrderId &&
                e.Id == key.Id);
    }

    public async Task<OrderItem?> GetItem(
        OrderItemKey key, CancellationToken ct = default)
    {
        return await _ctx.FindAsync<OrderItem>(key, ct);
    }

    public async Task<ResultObject> DeleteItem(OrderItemKey key)
    {
        var query = QuerySingleItem(key);

        var affected = await query.ExecuteDeleteAsync();

        if (affected == 0)
            return ResultObject.Fail(ResultError.NotFound,
                "Order item not found.");

        return ResultObject.Success();
    }
}