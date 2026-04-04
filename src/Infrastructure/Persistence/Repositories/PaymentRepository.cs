namespace FoodSphere.Infrastructure.Repository;

public class PaymentRepository(
    FoodSphereDbContext context,
    IPublishEndpoint publishEndpoint
) : RepositoryBase(context)
{
    public async Task<ResultObject<CashPayment>> CreateCashPayment(
        BillKey billKey,
        decimal amount,
        CancellationToken ct = default)
    {
        var bill = await _ctx.FindAsync<Bill>(billKey, ct);

        if (bill is null)
            return ResultObject.NotFound(billKey);

        int lastId;
        var hasPendingAdds = _ctx.ChangeTracker.Entries<Payment>()
            .Any(e =>
                e.State == EntityState.Added &&
                e.Entity.BillId == bill.Id);

        if (hasPendingAdds)
        {
            lastId = _ctx.Set<Payment>().Local
                .Where(e => e.BillId == bill.Id)
                .Max(e => (int?)e.Id) ?? 0;
        }
        else
        {
            lastId = await _ctx.Set<Payment>()
                .Where(e => e.BillId == bill.Id)
                .MaxAsync(e => (int?)e.Id, ct) ?? 0;
        }

        var payment = new CashPayment
        {
            Id = (short)(lastId + 1),
            BillId = billKey.Id,
            Amount = amount,
            Status = PaymentStatus.Succeeded,
        };

        await publishEndpoint.Publish(
            new PaymentCreatedMessage
            {
                Resource = payment,
                Branch = new BranchKey(bill.RestaurantId, bill.BranchId),
            },
            ct);

        await publishEndpoint.Publish(
            new PaymentStatusUpdatedMessage
            {
                Resource = payment,
                Status = payment.Status,
            },
            ct);

        _ctx.Add(payment);

        return payment;
    }

    public async Task<ResultObject<StripePayment>> CreateStripePayment(
        BillKey billKey,
        decimal amount,
        string sessionId,
        CancellationToken ct = default)
    {
        var bill = await _ctx.FindAsync<Bill>(billKey, ct);

        if (bill is null)
            return ResultObject.NotFound(billKey);

        int lastId;
        var hasPendingAdds = _ctx.ChangeTracker.Entries<Payment>()
            .Any(e =>
                e.State == EntityState.Added &&
                e.Entity.BillId == bill.Id);

        if (hasPendingAdds)
        {
            lastId = _ctx.Set<Payment>().Local
                .Where(e => e.BillId == bill.Id)
                .Max(e => (int?)e.Id) ?? 0;
        }
        else
        {
            lastId = await _ctx.Set<Payment>()
                .Where(e => e.BillId == bill.Id)
                .MaxAsync(e => (int?)e.Id, ct) ?? 0;
        }

        var payment = new StripePayment
        {
            Id = (short)(lastId + 1),
            BillId = billKey.Id,
            Amount = amount,
            SessionId = sessionId,
            Status = PaymentStatus.Pending,
        };

        await publishEndpoint.Publish(
            new PaymentCreatedMessage
            {
                Resource = payment,
                Branch = new BranchKey(bill.RestaurantId, bill.BranchId),
            },
            ct);

        _ctx.Add(payment);

        return payment;
    }

    public IQueryable<TPayment> QueryPayments<TPayment>()
        where TPayment : Payment
    {
        return _ctx.Set<TPayment>()
            .AsExpandable();
    }

    public IQueryable<Payment> QueryPayments()
        => QueryPayments<Payment>();

    public IQueryable<TPayment> QuerySinglePayment<TPayment>(PaymentKey key)
        where TPayment : Payment
    {
        return QueryPayments<TPayment>()
            .Where(e =>
                e.BillId == key.BillId &&
                e.Id == key.Id);
    }

    public IQueryable<Payment> QuerySinglePayment(PaymentKey key)
        => QuerySinglePayment<Payment>(key);


    public async Task<TPayment?> GetPayment<TPayment>(
        PaymentKey key, CancellationToken ct = default)
        where TPayment : Payment
    {
        return await _ctx.FindAsync<TPayment>(key, ct);
    }

    public async Task<Payment?> GetPayment(
        PaymentKey key, CancellationToken ct = default)
    {
        return await GetPayment<Payment>(key, ct);
    }
}