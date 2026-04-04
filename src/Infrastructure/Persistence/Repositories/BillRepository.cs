namespace FoodSphere.Infrastructure.Repository;

public class BillRepository(
    FoodSphereDbContext context,
    IPublishEndpoint publishEndpoint
) : RepositoryBase(context)
{
    public async Task<ResultObject<Bill>> CreateBill(
        TableKey tableKey,
        short? pax,
        ConsumerUserKey? consumerKey,
        CancellationToken ct = default)
    {
        var table = await _ctx.FindAsync<Table>(tableKey, ct);

        if (table is null)
            return ResultObject.NotFound(tableKey);

        var bill = new Bill
        {
            Id = Guid.CreateVersion7(),
            RestaurantId = tableKey.RestaurantId,
            BranchId = tableKey.BranchId,
            TableId = tableKey.Id,
            ConsumerId = consumerKey?.Id,
            Pax = pax,
        };

        _ctx.Add(bill);

        await publishEndpoint.Publish(
            new BillCreatedMessage
            {
                Resource = bill,
                Branch = new(bill.RestaurantId, bill.BranchId),
            },
            ct);

        return bill;
    }

    public IQueryable<Bill> QueryBills()
    {
        return _ctx.Set<Bill>()
            .AsExpandable();
    }

    public IQueryable<Bill> QuerySingleBill(BillKey key)
    {
        return QueryBills()
            .Where(e => e.Id == key.Id);
    }

    public async Task<Bill?> GetBill(
        BillKey key, CancellationToken ct = default)
    {
        return await _ctx.FindAsync<Bill>(key, ct);
    }
}