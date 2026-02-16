namespace FoodSphere.Common.Service;

public class BillService(FoodSphereDbContext context) : ServiceBase(context)
{
    public async Task<Bill> CreateBill(
        Guid restaurantId,
        short branchId,
        short tableId,
        short? pax,
        Guid? consumerId,
        CancellationToken ct = default
    ) {
        var bill = new Bill
        {
            RestaurantId = restaurantId,
            BranchId = branchId,
            TableId = tableId,
            Pax = pax,
            ConsumerId = consumerId,
        };

        _ctx.Add(bill);

        return bill;
    }

    public async Task<Bill> CreateBill(
        Table table,
        short? pax,
        Guid? consumerId,
        CancellationToken ct = default
    ) {
        return await CreateBill(
            table.RestaurantId,
            table.BranchId,
            table.Id,
            pax,
            consumerId,
            ct);
    }

    public Bill GetBillStub(Guid billId)
    {
        var bill = new Bill
        {
            Id = billId,
            // RestaurantId = restaurantId,
            // BranchId = branchId,
            // TableId = tableId,
        };

        _ctx.Attach(bill);

        return bill;
    }

    public IQueryable<Bill> QueryBills()
    {
        return _ctx.Set<Bill>()
            .AsExpandable();
    }

    public IQueryable<Bill> QuerySingleBill(Guid billId)
    {
        return QueryBills()
            .Where(e => e.Id == billId);
    }

    public async Task<Bill?> GetBill(Guid billId)
    {
        var existed = await _ctx.Set<Bill>()
            .AnyAsync(e => e.Id == billId);

        if (!existed)
        {
            return null;
        }

        return GetBillStub(billId);
    }

    public async Task<TDto?> GetBill<TDto>(Guid billId, Expression<Func<Bill, TDto>> projection)
    {
        return await QueryBills()
            .Where(e => e.Id == billId)
            .Select(projection)
            .SingleOrDefaultAsync();
    }

    public async Task DeleteBill(Bill bill)
    {
        if (bill.Status == BillStatus.Pending)
        {
            _ctx.Remove(bill);
        }
    }

    public async Task<Order> CreateOrder(Bill bill, CancellationToken ct = default)
    {
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
            Bill = bill
        };

        _ctx.Add(order);

        return order;
    }

    public Order GetOrderStub(Guid billId, short orderId)
    {
        var order = new Order
        {
            BillId = billId,
            Id = orderId
        };

        _ctx.Attach(order);

        return order;
    }

    public IQueryable<Order> QueryOrders()
    {
        return _ctx.Set<Order>()
            .AsExpandable();
    }

    public IQueryable<Order> QueryOrders(Guid billId)
    {
        return QueryOrders()
            .Where(e => e.BillId == billId);
    }

    public IQueryable<Order> QuerySingleOrder(Guid billId, short orderId)
    {
        return QueryOrders(billId)
            .Where(e => e.Id == orderId);
    }

    public async Task<TDto?> GetOrder<TDto>(Guid billId, short orderId, Expression<Func<Order, TDto>> projection)
    {
        return await QuerySingleOrder(billId, orderId)
            .Select(projection)
            .SingleOrDefaultAsync();
    }

    public async Task<Order?> GetOrder(Guid billId, short orderId)
    {
        var existed = await _ctx.Set<Order>()
            .AnyAsync(e =>
                e.BillId == billId &&
                e.Id == orderId);

        if (!existed)
        {
            return null;
        }

        return GetOrderStub(billId, orderId);
    }

    public async Task DeleteOrder(Order order)
    {
        if (order.Status == OrderStatus.Pending)
        {
            _ctx.Remove(order);
        }
    }

    public async Task<BillMember> CreateMember(
        Guid billId,
        Guid? consumerId,
        string? name = null,
        CancellationToken ct = default
    ) {
        int lastId;
        var hasPendingAdds = _ctx.ChangeTracker.Entries<BillMember>()
            .Any(e =>
                e.State == EntityState.Added &&
                e.Entity.BillId == billId);

        if (hasPendingAdds)
        {
            lastId = _ctx.Set<BillMember>().Local
                .Where(e => e.BillId == billId)
                .Max(e => (int?)e.Id) ?? 0;
        }
        else
        {
            lastId = await _ctx.Set<BillMember>()
                .Where(e => e.BillId == billId)
                .MaxAsync(e => (int?)e.Id, ct) ?? 0;
        }

        var member = new BillMember()
        {
            Id = (short)(lastId + 1),
            BillId = billId,
            ConsumerId = consumerId,
            Name = name,
        };

        _ctx.Add(member);

        return member;
    }

    public BillMember GetMemberStub(Guid billId, short memberId)
    {
        var member = new BillMember
        {
            BillId = billId,
            Id = memberId,
        };

        _ctx.Attach(member);

        return member;
    }

    public IQueryable<BillMember> QueryMembers()
    {
        return _ctx.Set<BillMember>()
            .AsExpandable();
    }

    public IQueryable<BillMember> QueryMembers(Guid billId)
    {
        return QueryMembers()
            .Where(e =>
                e.BillId == billId);
    }

    public IQueryable<BillMember> QuerySingleMember(Guid billId, short memberId)
    {
        return QueryMembers(billId)
            .Where(e => e.Id == memberId);
    }

    public async Task<TDto?> GetMember<TDto>(Guid billId, short memberId, Expression<Func<BillMember, TDto>> projection)
    {
        return await QuerySingleMember(billId, memberId)
            .Select(projection)
            .SingleOrDefaultAsync();
    }

    public async Task<BillMember?> GetMember(Guid billId, short memberId)
    {
        var existed = await _ctx.Set<BillMember>()
            .AnyAsync(e =>
                e.BillId == billId &&
                e.Id == memberId);

        if (!existed)
        {
            return null;
        }

        return GetMemberStub(billId, memberId);
    }

    public async Task<OrderItem> CreateItem(
        Order order,
        Menu menu,
        short quantity,
        string? note,
        CancellationToken ct = default
    ) {
        int lastId;
        var hasPendingAdds = _ctx.ChangeTracker.Entries<OrderItem>()
            .Any(e =>
                e.State == EntityState.Added &&
                e.Entity.BillId == order.BillId &&
                e.Entity.OrderId == order.Id);

        if (hasPendingAdds)
        {
            lastId = _ctx.Set<OrderItem>().Local
                .Where(e => e.BillId == order.BillId && e.OrderId == order.Id)
                .Max(e => (int?)e.Id) ?? 0;
        }
        else
        {
            lastId = await _ctx.Set<OrderItem>()
                .Where(e => e.BillId == order.BillId && e.OrderId == order.Id)
                .MaxAsync(e => (int?)e.Id, ct) ?? 0;
        }

        var item = new OrderItem
        {
            Id = (short)(lastId + 1),
            Order = order,
            Menu = menu,
            PriceSnapshot = menu.Price,
            Quantity = quantity,
            Note = note,
        };

        _ctx.Add(item);

        return item;
    }

    public OrderItem GetItemStub(Guid billId, short orderId, short itemId)
    {
        var item = new OrderItem
        {
            BillId = billId,
            OrderId = orderId,
            Id = itemId,
        };

        _ctx.Attach(item);

        return item;
    }

    public IQueryable<OrderItem> QueryItems()
    {
        return _ctx.Set<OrderItem>()
            .AsExpandable();
    }

    public IQueryable<OrderItem> QueryItems(Guid billId, short orderId)
    {
        return QueryItems()
            .Where(e =>
                e.BillId == billId &&
                e.OrderId == orderId);
    }

    public IQueryable<OrderItem> QuerySingleItem(Guid billId, short orderId, short itemId)
    {
        return QueryItems(billId, orderId)
            .Where(e =>
                e.Id == itemId);
    }

    public async Task<TDto?> GetItem<TDto>(Guid billId, short orderId, short itemId, Expression<Func<OrderItem, TDto>> projection)
    {
        return await QuerySingleItem(billId, orderId, itemId)
            .Select(projection)
            .SingleOrDefaultAsync();
    }

    public async Task<OrderItem?> GetItem(Guid billId, short orderId, short itemId)
    {
        var existed = await _ctx.Set<OrderItem>()
            .AnyAsync(e =>
                e.BillId == billId &&
                e.OrderId == orderId &&
                e.Id == itemId);

        if (!existed)
        {
            return null;
        }

        return GetItemStub(billId, orderId, itemId);
    }

    // public async Task UpdateItem(
    //     OrderItem item,

    //     CancellationToken ct = default
    // ) {
    //     if (quantity is not null)
    //     {
    //         ArgumentOutOfRangeException.ThrowIfNegative(quantity.Value);
    //         item.Quantity = quantity.Value;
    //     }

    //     item.Note ??= note;

    //     // _ctx.Entry(orderItem).State = EntityState.Modified;
    // }
}