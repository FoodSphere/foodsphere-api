namespace FoodSphere.Infrastructure.Repository;

public class BillMemberRepository(
    FoodSphereDbContext context,
    IPublishEndpoint publishEndpoint
) : RepositoryBase(context)
{
    public async Task<ResultObject<BillMember>> CreateMember(
        BillKey billKey,
        string? name,
        ConsumerUserKey? consumerKey,
        CancellationToken ct = default)
    {
        int lastId;
        var hasPendingAdds = _ctx.ChangeTracker.Entries<BillMember>()
            .Any(e =>
                e.State == EntityState.Added &&
                e.Entity.BillId == billKey.Id);

        if (hasPendingAdds)
        {
            lastId = _ctx.Set<BillMember>().Local
                .Where(e => e.BillId == billKey.Id)
                .Max(e => (int?)e.Id) ?? 0;
        }
        else
        {
            lastId = await _ctx.Set<BillMember>()
                .Where(e => e.BillId == billKey.Id)
                .MaxAsync(e => (int?)e.Id, ct) ?? 0;
        }

        var member = new BillMember()
        {
            Id = (short)(lastId + 1),
            BillId = billKey.Id,
            ConsumerId = consumerKey?.Id,
            Name = name,
        };

        _ctx.Add(member);

        return member;
    }

    BillMember CreateMemberStub(BillMemberKey key)
    {
        var member = new BillMember
        {
            Id = key.Id,
            BillId = key.BillId,
        };

        _ctx.Attach(member);

        return member;
    }

    public IQueryable<BillMember> QueryMembers()
    {
        return _ctx.Set<BillMember>()
            .AsExpandable();
    }

    public IQueryable<BillMember> QuerySingleMember(
        BillMemberKey key)
    {
        return QueryMembers()
            .Where(e =>
                e.BillId == key.BillId &&
                e.Id == key.Id);
    }

    public async Task<BillMember?> GetMember(
        BillMemberKey key, CancellationToken ct = default)
    {
        return await _ctx.FindAsync<BillMember>(key, ct);
    }

    public async Task<ResultObject> DeleteMember(
        BillMemberKey key, CancellationToken ct = default)
    {
        var query = QuerySingleMember(key);

        var affected = await query.ExecuteDeleteAsync(ct);

        if (affected == 0)
            return ResultObject.Fail(ResultError.NotFound,
                "Bill member not found.");

        return ResultObject.Success();
    }
}