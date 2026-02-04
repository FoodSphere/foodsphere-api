using FoodSphere.Infrastructure.Persistence;

namespace FoodSphere.Pos.Api.Services;

public class StaffPortalService(
    FoodSphereDbContext context,
    StaffAuthService staffAuthService
) : ServiceBase(context)
{
    public async Task<StaffPortal?> GetStaffPortal(Guid portal_id)
    {
        var portal = await _ctx.FindAsync<StaffPortal>(portal_id);

        if (portal is null || !portal.IsValid())
        {
            return null;
        }

        return portal;
    }

    public async Task<string> GenerateToken(StaffPortal portal) {
        if (!portal.IsValid())
        {
            throw new Exception("staff link invalid.");
        }

        portal.Use();

        var token = await staffAuthService.GenerateToken(portal.StaffUser);

        return token;
    }

    public async Task<StaffPortal> CreateStaffPortal(Guid restaurantId, short branchId, short staffId)
    {
        var portal = new StaffPortal
        {
            RestaurantId = restaurantId,
            BranchId = branchId,
            StaffId = staffId,
        };

        await _ctx.AddAsync(portal);

        return portal;
    }

    public async Task<StaffPortal> CreateStaffPortal(StaffUser staff)
    {
        return await CreateStaffPortal(staff.RestaurantId, staff.BranchId, staff.Id);
    }
}