namespace FoodSphere.Common.Service;

public class OrderingPortalService(
    FoodSphereDbContext context,
    OrderingAuthService orderingAuthService,
    BillService billService
) : ServiceBase(context)
{
    public async Task<string> GenerateToken(
        SelfOrderingPortal portal,
        Guid? consumerId = null)
    {
        if (!portal.IsValid())
        {
            throw new Exception("Ordering link invalid.");
        }

        portal.Use();

        var member = await billService.CreateMember(portal.BillId, consumerId);
        var token = await orderingAuthService.GenerateToken(member);

        return token;
    }

    public async Task<SelfOrderingPortal> CreatePortal(
        Guid billId,
        short? maxUsage = null,
        // Guid? issuerId = null
        TimeSpan? validDuration = null)
    {
        var portal = new SelfOrderingPortal
        {
            BillId = billId,
            MaxUsage = maxUsage,
            // IssuerId = issuerId,
            ValidDuration = validDuration
        };

        _ctx.Add(portal);

        return portal;
    }

    public SelfOrderingPortal GetPortalStub(Guid portalId)
    {
        var portal = new SelfOrderingPortal
        {
            Id = portalId
        };

        _ctx.Attach(portal);

        return portal;
    }

    public IQueryable<SelfOrderingPortal> QueryPortals()
    {
        return _ctx.Set<SelfOrderingPortal>()
            .AsExpandable();
    }

    public IQueryable<SelfOrderingPortal> QuerySinglePortal(Guid portalId)
    {
        return QueryPortals()
            .Where(e =>
                e.Id == portalId);
    }

    public async Task<TDto?> GetPortal<TDto>(Guid portal_id, Expression<Func<SelfOrderingPortal, TDto>> projection)
    {
        return await QuerySinglePortal(portal_id)
            .Select(projection)
            .SingleOrDefaultAsync();
    }

    public async Task<SelfOrderingPortal?> GetPortal(Guid portal_id)
    {
        var existed = await _ctx.Set<SelfOrderingPortal>()
            .AnyAsync(e =>
                e.Id == portal_id);

        if (!existed)
        {
            return null;
        }

        return GetPortalStub(portal_id);
    }
}