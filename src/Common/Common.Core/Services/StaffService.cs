using Microsoft.EntityFrameworkCore;
using FoodSphere.Infrastructure.Persistence;

namespace FoodSphere.Common.Service;

public class StaffService(FoodSphereDbContext context) : ServiceBase(context)
{
    public async Task<StaffUser> CreateStaffAsync(
        Branch branch,
        string name,
        string? phone = null,
        CancellationToken ct = default
    ) {
        return await CreateStaffAsync(
            branch.RestaurantId,
            branch.Id,
            name,
            phone,
            ct);
    }

    public async Task<StaffUser> CreateStaffAsync(
        Guid restaurantId,
        short branchId,
        string name,
        string? phone = null,
        CancellationToken ct = default
    ) {
        var lastId = await _ctx.Set<StaffUser>()
            .Where(staff => staff.RestaurantId == restaurantId && staff.BranchId == branchId)
            .MaxAsync(staff => (short?)staff.Id, ct) ?? 0;

        var staff = new StaffUser
        {
            Id = (short)(lastId + 1),
            RestaurantId = restaurantId,
            BranchId = branchId,
            Name = name,
            Phone = phone
        };

        await _ctx.AddAsync(staff, ct);

        return staff;
    }

    public async Task SetRolesAsync(
        StaffUser staff,
        IEnumerable<short> roleIds,
        CancellationToken ct = default
    ) {
        await SetRolesAsync(
            staff.RestaurantId,
            staff.BranchId,
            staff.Id,
            roleIds,
            ct);
    }

    public async Task SetRolesAsync(
        Guid restaurantId,
        short branchId,
        short staffId,
        IEnumerable<short> roleIds,
        CancellationToken ct = default
    ) {
        var desiredIds = roleIds
            .Distinct()
            .ToArray();

        var currentRoles = await _ctx.Set<StaffRole>()
            .Where(sr =>
                sr.RestaurantId == restaurantId &&
                sr.BranchId == branchId &&
                sr.StaffId == staffId)
            .ToArrayAsync(ct);

        var toRemove = currentRoles
            .ExceptBy(desiredIds, sr => sr.RoleId)
            .ToArray();

        var toAddIds = desiredIds
            .Except(currentRoles.Select(sr => sr.RoleId))
            .ToArray();

        var newEntities = toAddIds.Select(roleId => new StaffRole
        {
            RestaurantId = restaurantId,
            BranchId = branchId,
            StaffId = staffId,
            RoleId = roleId
        });

        _ctx.RemoveRange(toRemove);
        await _ctx.AddRangeAsync(newEntities, ct);
    }

    public async Task<StaffUser?> GetStaff(Guid restaurantId, short branchId, short staffId)
    {
        return await _ctx.FindAsync<StaffUser>(restaurantId, branchId, staffId);
    }

    public async Task<List<StaffUser>> ListStaffs(Guid restaurantId, short branchId)
    {
        return await _ctx.Set<StaffUser>()
            .Where(staff => staff.RestaurantId == restaurantId && staff.BranchId == branchId)
            .ToListAsync();
    }

    public async Task DeleteStaff(StaffUser staff)
    {
        _ctx.Remove(staff);
    }

}