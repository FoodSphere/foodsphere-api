namespace FoodSphere.Infrastructure.Repository;

public class TagRepository(
    FoodSphereDbContext context
) : RepositoryBase(context)
{
    public async Task<ResultObject<Tag>> CreateTag(
        RestaurantKey restaurantKey,
        string name, string? type,
        CancellationToken ct = default)
    {
        int lastId;
        var hasPendingAdds = _ctx.ChangeTracker.Entries<Tag>()
            .Any(e =>
                e.State == EntityState.Added &&
                e.Entity.RestaurantId == restaurantKey.Id);

        if (hasPendingAdds)
        {
            lastId = _ctx.Set<Tag>().Local
                .Where(tag => tag.RestaurantId == restaurantKey.Id)
                .Max(tag => (int?)tag.Id) ?? 0;
        }
        else
        {
            lastId = await _ctx.Set<Tag>()
                .Where(tag => tag.RestaurantId == restaurantKey.Id)
                .Select(tag => (int?)tag.Id)
                .MaxAsync(ct) ?? 0;
        }

        var tag = new Tag
        {
            Id = (short)(lastId + 1),
            RestaurantId = restaurantKey.Id,
            Name = name
        };

        _ctx.Add(tag);

        return tag;
    }

    public IQueryable<Tag> QueryTags()
    {
        return _ctx.Set<Tag>()
            .AsExpandable();
    }

    public IQueryable<Tag> QuerySingleTag(TagKey key)
    {
        return _ctx.Set<Tag>()
            .Where(tag =>
                tag.RestaurantId == key.RestaurantId &&
                tag.Id == key.Id);
    }

    public async Task<Tag?> GetTag(
        TagKey key, CancellationToken ct = default)
    {
        return await _ctx.FindAsync<Tag>(key, ct);
    }

    public async Task<ResultObject> DeleteTag(
        TagKey key, CancellationToken ct = default)
    {
        var tag = await GetTag(key, ct);

        if (tag is null)
            return ResultObject.NotFound(key);

        _ctx.Remove(tag);

        return ResultObject.Success();
    }
}