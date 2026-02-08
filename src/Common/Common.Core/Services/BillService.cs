using Microsoft.EntityFrameworkCore;
using FoodSphere.Infrastructure.Persistence;

namespace FoodSphere.Common.Service;

public class BillService(FoodSphereDbContext context) : ServiceBase(context)
{
    public async Task<List<Bill>> ListBills()
    {
        return await _ctx.Set<Bill>()
            .ToListAsync();
    }

    public async Task<Bill?> GetBill(Guid billId)
    {
        return await _ctx.FindAsync<Bill>(billId);
    }

    public async Task<Order?> GetOrder(Guid billId, short orderId)
    {
        return await _ctx.FindAsync<Order>(billId, orderId);
    }

    public async Task<List<Order>> ListOrders(Guid billId)
    {
        return await _ctx.Set<Order>()
            .Where(order => order.BillId == billId)
            .ToListAsync();
    }

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

        await _ctx.AddAsync(bill, ct);

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

    public async Task<Order> CreateOrder(Bill bill, CancellationToken ct = default)
    {
        var lastId = await _ctx.Set<Order>()
            .Where(order => order.BillId == bill.Id)
            .MaxAsync(order => (short?)order.Id, ct) ?? 0;

        var order = new Order
        {
            Id = (short)(lastId + 1),
            Bill = bill
        };

        bill.Orders.Add(order);

        return order;
    }

    public async Task<BillMember> AddBillMember(
        Guid billId,
        Guid? consumerId,
        string? name = null
    ) {
        var lastId = await _ctx.Set<BillMember>()
            .Where(member => member.BillId == billId)
            .MaxAsync(order => (int?)order.Id) ?? 0;

        var member = new BillMember()
        {
            Id = (short)(lastId + 1),
            BillId = billId,
            ConsumerId = consumerId,
            Name = name
        };

        await _ctx.AddAsync(member);

        return member;
    }

    public async Task<BillMember?> GetBillMember(Guid billId, short memberId)
    {
        return await _ctx.FindAsync<BillMember>(billId, memberId);
    }

    public async Task<List<BillMember>> ListBillMembers(Guid billId)
    {
        return await _ctx.Set<BillMember>()
            .Where(member => member.BillId == billId)
            .ToListAsync();
    }

    public async Task DeleteBill(Bill bill)
    {
        if (bill.Status == BillStatus.Pending)
        {
            _ctx.Remove(bill);
        }
    }

    public async Task CreateOrderItem(
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
                .Where(item => item.BillId == order.BillId && item.OrderId == order.Id)
                .Max(item => (int?)item.Id) ?? 0;
        }
        else
        {
            lastId = await _ctx.Set<OrderItem>()
                .Where(item => item.BillId == order.BillId && item.OrderId == order.Id)
                .Select(item => (int?)item.Id)
                .MaxAsync(ct) ?? 0;
        }

        var item = new OrderItem
        {
            Id = (short)(lastId + 1),
            Order = order,
            Menu = menu,
            PriceSnapshot = menu.Price,
            Quantity = quantity,
            Note = note
        };

        order.Items.Add(item);
    }

    // public async Task UpdateOrderItem(
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

    public async Task UpdateOrderStatus(Order order, OrderStatus status)
    {
        order.Status = status;
        // _ctx.Entry(order).State = EntityState.Modified;
    }

    public async Task DeleteOrder(Order order)
    {
        if (order.Status == OrderStatus.Pending)
        {
            _ctx.Remove(order);
        }
    }

    public async Task<bool> CheckPermissions(Guid billId, Guid consumerId)
    {
        return await _ctx.Set<Bill>()
            .AnyAsync(b =>
                b.Id == billId &&
                b.ConsumerId == consumerId
            );
    }

    // public async Task<bool> CheckPermissions(Bill bill, MasterUser user, Permission[]? permissions = null)
    // {
    //     return await _ctx.Set<Restaurant>()
    //         .AnyAsync(r =>
    //             r.Id == bill.RestaurantId && (
    //                 r.OwnerId == user.Id ||
    //                 _ctx.Set<BranchManager>().Any(m =>
    //                     m.RestaurantId == r.Id &&
    //                     m.BranchId == bill.BranchId &&
    //                     m.MasterId == user.Id
    //     )));
    // }

    // public async Task<bool> CheckPermissions(Bill bill, StaffUser user, Permission[]? permissions = null)
    // {
    //     return bill.RestaurantId == user.RestaurantId &&
    //            bill.BranchId == user.BranchId;
    // }

    // public async Task<bool> CheckPermissions(Bill bill, ConsumerUser user)
    // {
    //     return bill.ConsumerId == user.Id;
    // }
}