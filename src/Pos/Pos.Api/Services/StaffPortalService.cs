using FoodSphere.Infrastructure.Persistence;

namespace FoodSphere.Pos.Api.Service;

public class StaffPortalService(
    FoodSphereDbContext context,
    StaffAuthService staffAuthService
) : ServiceBase(context)
{
    public async Task<string> GenerateToken(StaffPortal portal) {
        if (!portal.IsValid())
        {
            throw new Exception("staff link invalid.");
        }

        portal.Use();

        var token = await staffAuthService.GenerateToken(portal.StaffUser);

        return token;
    }

    public async Task<StaffPortal> CreatePortal(
        Guid restaurantId,
        short branchId,
        short staffId,
        TimeSpan? validDuration = null,
        short maxUsage = 1
    ) {
        var portal = new StaffPortal
        {
            RestaurantId = restaurantId,
            BranchId = branchId,
            StaffId = staffId,
            ValidDuration = validDuration,
            MaxUsage = maxUsage,
        };

        _ctx.Add(portal);

        return portal;
    }

    public StaffPortal GetPortalStub(Guid portalId)
    {
        var portal = new StaffPortal
        {
            Id = portalId,
        };

        _ctx.Attach(portal);

        return portal;
    }

    public async Task<StaffPortal> CreatePortal(StaffUser staff, TimeSpan? validDuration = null)
    {
        return await CreatePortal(staff.RestaurantId, staff.BranchId, staff.Id, validDuration);
    }

    public IQueryable<StaffPortal> QueryPortals()
    {
        return _ctx.Set<StaffPortal>()
            .AsExpandable();
    }

    public IQueryable<StaffPortal> QuerySinglePortal(Guid portalId)
    {
        return QueryPortals()
            .Where(e => e.Id == portalId);
    }

    public async Task<TDto?> GetPortal<TDto>(Guid portal_id, Expression<Func<StaffPortal, TDto>> projection)
    {
        return await QuerySinglePortal(portal_id)
            .Select(projection)
            .SingleOrDefaultAsync();
    }

    public async Task<StaffPortal?> GetPortal(Guid portal_id)
    {
        return await _ctx.FindAsync<StaffPortal>(portal_id);
    }
}