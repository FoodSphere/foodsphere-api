using Microsoft.AspNetCore.Identity;

namespace FoodSphere.Infrastructure.Repository;

public class MasterRepository(
    FoodSphereDbContext context,
    IPasswordHasher<MasterUser> passwordHasher
) : RepositoryBase(context)
{
    public async Task<ResultObject<MasterUser>> CreateMaster(
        string email,
        string password,
        string userName,
        string? phoneNumber,
        bool twoFactorEnabled = false,
        CancellationToken ct = default)
    {
        var masterUser = new MasterUser
        {
            Email = email,
            UserName = userName,
            PasswordHash = default!,
            PhoneNumber = phoneNumber,
            TwoFactorEnabled = twoFactorEnabled
        };

        masterUser.PasswordHash = passwordHasher.HashPassword(masterUser, password);

        _ctx.Add(masterUser);

        return masterUser;
    }

    MasterUser CreateMasterStub(MasterUserKey key)
    {
        var masterUser = new MasterUser
        {
            Id = key.Id,
            Email = default!,
            UserName = default!,
            PasswordHash = default!,
        };

        _ctx.Attach(masterUser);

        return masterUser;
    }

    public IQueryable<MasterUser> QueryMasters()
    {
        return _ctx.Set<MasterUser>()
            .AsExpandable();
    }

    public IQueryable<MasterUser> QuerySingleMaster(MasterUserKey key)
    {
        return QueryMasters()
            .Where(u => u.Id == key.Id);
    }

    public async Task<MasterUser?> GetMaster(
        MasterUserKey key, CancellationToken ct = default)
    {
        return await _ctx.FindAsync<MasterUser>(key, ct);
    }

    public async Task<ResultObject> DeleteMaster(
        MasterUserKey key, CancellationToken ct = default)
    {
        var master = await GetMaster(key, ct);

        if (master is null)
            return ResultObject.NotFound(key);

        _ctx.Remove(master);

        return ResultObject.Success();
    }

    public async Task<TDto?> GetMaster<TDto>(
        Expression<Func<MasterUser, TDto>> projection,
        MasterUserKey key,
        CancellationToken ct = default)
    {
        return await QuerySingleMaster(key)
            .Select(projection)
            .SingleOrDefaultAsync(ct);
    }
}