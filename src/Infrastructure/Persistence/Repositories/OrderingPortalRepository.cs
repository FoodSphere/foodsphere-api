namespace FoodSphere.Infrastructure.Repository;

public class OrderingPortalRepository(
    FoodSphereDbContext context,
    BillRepository billRepository
) : RepositoryBase(context)
{
    public async Task<ResultObject<OrderingPortal>> CreatePortal(
        BillKey billKey,
        short? maxUsage,
        // Guid? issuerId
        TimeSpan? validDuration,
        CancellationToken ct = default)
    {
        var bill = await billRepository.GetBill(billKey, ct);

        if (bill is null)
            return ResultObject.NotFound(billKey);

        if (bill.Status is not BillStatus.Open)
            return ResultObject.Fail(ResultError.State,
                "Only portals for open bills can be created.");

        var portal = new OrderingPortal
        {
            Id = Guid.NewGuid(),
            BillId = billKey.Id,
            MaxUsage = maxUsage,
            // IssuerId = issuerId,
            ValidDuration = validDuration
        };

        _ctx.Add(portal);

        return portal;
    }

    OrderingPortal CreatePortalStub(PortalKey key)
    {
        var portal = new OrderingPortal
        {
            Id = key.Id,
            BillId = default,
        };

        _ctx.Attach(portal);

        return portal;
    }

    public IQueryable<OrderingPortal> QueryPortals()
    {
        return _ctx.Set<OrderingPortal>()
            .AsExpandable();
    }

    public IQueryable<OrderingPortal> QuerySinglePortal(PortalKey key)
    {
        return QueryPortals()
            .Where(e =>
                e.Id == key.Id);
    }

    public async Task<TDto?> GetPortal<TDto>(
        PortalKey key, Expression<Func<OrderingPortal, TDto>> projection,
        CancellationToken ct = default)
    {
        return await QuerySinglePortal(key)
            .Select(projection)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<OrderingPortal?> GetPortal(PortalKey key, CancellationToken ct = default)
    {
        return await _ctx.FindAsync<OrderingPortal>(key, ct);
    }
}