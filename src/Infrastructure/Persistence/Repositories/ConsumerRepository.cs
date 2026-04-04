using Microsoft.AspNetCore.Identity;

namespace FoodSphere.Infrastructure.Repository;

public class ConsumerRepository(
    FoodSphereDbContext context,
    IPasswordHasher<ConsumerUser> passwordHasher
) : RepositoryBase(context)
{
    public async Task<ResultObject<ConsumerUser>> CreateConsumer(
        string email,
        string password,
        string username,
        string? phoneNumber,
        bool twoFactorEnabled = false,
        CancellationToken ct = default)
    {
        var consumer = new ConsumerUser
        {
            Id = Guid.CreateVersion7(),
            Email = email,
            UserName = username,
            PasswordHash = default!,
            PhoneNumber = phoneNumber,
            TwoFactorEnabled = twoFactorEnabled
        };

        consumer.PasswordHash = passwordHasher.HashPassword(consumer, password);

        _ctx.Add(consumer);

        return consumer;
    }

    ConsumerUser CreateConsumerStub(ConsumerUserKey key)
    {
        var consumer = new ConsumerUser
        {
            Id = key.Id,
            Email = default!,
            UserName = default!,
            PasswordHash = default!,
        };

        _ctx.Attach(consumer);

        return consumer;
    }

    public IQueryable<ConsumerUser> QueryConsumers()
    {
        return _ctx.Set<ConsumerUser>()
            .AsExpandable();
    }

    public IQueryable<ConsumerUser> QuerySingleConsumer(ConsumerUserKey key)
    {
        return QueryConsumers()
            .Where(u => u.Id == key.Id);
    }

    public async Task<TDto?> GetConsumer<TDto>(
        Expression<Func<ConsumerUser, TDto>> projection,
        ConsumerUserKey key,
        CancellationToken ct = default)
    {
        return await QuerySingleConsumer(key)
            .Select(projection)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<ResultObject> DeleteConsumer(
        ConsumerUserKey key, CancellationToken ct = default)
    {
        var consumer = await QuerySingleConsumer(key)
            .SingleOrDefaultAsync(ct);

        if (consumer is null)
            return ResultObject.NotFound(key);

        _ctx.Remove(consumer);

        return ResultObject.Success();
    }
}