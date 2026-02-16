namespace FoodSphere.Common.Service;

public class ConsumerService(FoodSphereDbContext context) : ServiceBase(context){
    public async Task<ConsumerUser> CreateConsumer(
        string email,
        string password,
        string? name = null,
        string? phone = null,
        bool twoFactorEnabled = false,
        CancellationToken ct = default)
    {
        var consumer = new ConsumerUser
        {
            Email = email,
            PasswordHash = password, // not yet hashed
            Name = name,
            Phone = phone,
            TwoFactorEnabled = twoFactorEnabled
        };

        _ctx.Add(consumer);

        return consumer;
    }

    public ConsumerUser GetConsumerStub(Guid consumerId)
    {
        var consumer = new ConsumerUser
        {
            Id = consumerId,
            Email = default!,
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

    public IQueryable<ConsumerUser> QuerySingleConsumer(Guid userId)
    {
        return QueryConsumers()
            .Where(u => u.Id == userId);
    }

    public async Task<TDto?> GetConsumer<TDto>(Guid consumerId, Expression<Func<ConsumerUser, TDto>> projection)
    {
        return await QuerySingleConsumer(consumerId)
            .Select(projection)
            .SingleOrDefaultAsync();
    }

    public async Task<ConsumerUser?> GetConsumer(Guid consumerId)
    {
        var existed = await _ctx.Set<ConsumerUser>()
            .AnyAsync(c => c.Id == consumerId);

        if (!existed)
        {
            return null;
        }

        return GetConsumerStub(consumerId);
    }
}