using FoodSphere.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodSphere.Services;

public class StaffService(
    AppDbContext context,
    StaffAuthService staffAuthService
) : BaseService(context)
{
    readonly StaffAuthService _staffAuthService = staffAuthService;

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

        var token = await _staffAuthService.GenerateToken(portal.StaffUser);

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

    public async Task<StaffUser> CreateStaff(
        Branch branch,
        string name,
        List<short> roles,
        string? phone = null
    ) {
        var lastId = await _ctx.Set<StaffUser>()
            .Where(staff => staff.RestaurantId == branch.RestaurantId && staff.BranchId == branch.Id)
            .MaxAsync(staff => (int?)staff.Id) ?? 0;

        var staff = new StaffUser
        {
            Id = (short)(lastId + 1),
            Branch = branch,
            Name = name,
            Phone = phone
        };

        var rolesModel = await _ctx.Set<Role>()
            .Where(role => roles.Contains(role.Id))
            .ToArrayAsync();

        // staff.Roles.AddRange(rolesModel);

        await _ctx.AddAsync(staff);

        return staff;
    }

    public async Task<StaffUser?> GetStaff(Guid restaurantId, short branchId, short staffId)
    {
        return await _ctx.FindAsync<StaffUser>(restaurantId, branchId, staffId);
    }

    public async Task DeleteStaff(StaffUser staff)
    {
        _ctx.Remove(staff);
    }

}