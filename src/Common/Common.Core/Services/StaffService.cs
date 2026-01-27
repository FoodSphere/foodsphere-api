using Microsoft.EntityFrameworkCore;
using FoodSphere.Infrastructure.Persistence;

namespace FoodSphere.Common.Services;

public class StaffService(FoodSphereDbContext context) : ServiceBase(context)
{
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