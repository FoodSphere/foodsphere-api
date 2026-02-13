using Microsoft.EntityFrameworkCore;
using FoodSphere.Infrastructure.Persistence;

namespace FoodSphere.Common.Service;

public class StaffService(FoodSphereDbContext context) : ServiceBase(context)
{
    public async Task<StaffUser> CreateStaff(
        Branch branch,
        string name,
        string? phone = null,
        CancellationToken ct = default
    ) {
        return await CreateStaff(
            branch.RestaurantId,
            branch.Id,
            name,
            phone,
            ct);
    }

    public async Task<StaffUser> CreateStaff(
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

        _ctx.Add(staff);

        return staff;
    }

    public async Task SetRoles(
        StaffUser staff,
        IEnumerable<short> roleIds,
        CancellationToken ct = default
    ) {
        await SetRoles(
            staff.RestaurantId,
            staff.BranchId,
            staff.Id,
            roleIds,
            ct);
    }

    public async Task SetRoles(
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

    public async Task<StaffUser?> GetDefaultStaff(Guid restaurantId, short staffId)
    {
        return await _ctx.FindAsync<StaffUser>(restaurantId, 1, staffId);
    }

    public async Task<StaffUser[]> ListStaffs(Guid restaurantId, short branchId)
    {
        return await _ctx.Set<StaffUser>()
            .Where(staff => staff.RestaurantId == restaurantId && staff.BranchId == branchId)
            .ToArrayAsync();
    }

    public async Task<StaffUser[]> ListDefaultStaffs(Guid restaurantId)
    {
        return await _ctx.Set<StaffUser>()
            .Where(staff => staff.RestaurantId == restaurantId && staff.BranchId == 1)
            .ToArrayAsync();
    }

    public async Task DeleteStaff(StaffUser staff)
    {
        _ctx.Remove(staff);
    }
}