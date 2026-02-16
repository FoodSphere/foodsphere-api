using Microsoft.EntityFrameworkCore;
using FoodSphere.Infrastructure.Persistence;

namespace FoodSphere.Common.Service;

public class TagService(FoodSphereDbContext context) : ServiceBase(context)
{
    public async Task<Tag> CreateTag(
        Restaurant restaurant,
        string name,
        CancellationToken ct = default
    ) {
        return await CreateTag(
            restaurant.Id,
            name,
            ct);
    }

    public async Task<Tag> CreateTag(
        Guid restaurantId,
        string name,
        CancellationToken ct = default
    ) {
        int lastId;
        var hasPendingAdds = _ctx.ChangeTracker.Entries<Tag>()
            .Any(e => e.State == EntityState.Added && e.Entity.RestaurantId == restaurantId);

        if (hasPendingAdds)
        {
            lastId = _ctx.Set<Tag>().Local
                .Where(tag => tag.RestaurantId == restaurantId)
                .Max(tag => (int?)tag.Id) ?? 0;
        }
        else
        {
            lastId = await _ctx.Set<Tag>()
                .Where(tag => tag.RestaurantId == restaurantId)
                .Select(tag => (int?)tag.Id)
                .MaxAsync(ct) ?? 0;
        }

        var tag = new Tag
        {
            Id = (short)(lastId + 1),
            RestaurantId = restaurantId,
            Name = name,
        };

        _ctx.Add(tag);

        return tag;
    }

    public Tag GetTagStub(Guid restaurantId, short tagId)
    {
        var tag = new Tag
        {
            Id = tagId,
            RestaurantId = restaurantId,
            Name = default!,
        };

        _ctx.Attach(tag);

        return tag;
    }

    public IQueryable<Tag> QueryTags()
    {
        return _ctx.Set<Tag>()
            .AsExpandable();
    }

    public IQueryable<Tag> QuerySingleTag(Guid restaurantId, short tagId)
    {
        return _ctx.Set<Tag>()
            .Where(tag =>
                tag.RestaurantId == restaurantId &&
                tag.Id == tagId);
    }

    public async Task<TDto?> GetTag<TDto>(Guid restaurantId, short tagId, Expression<Func<Tag, TDto>> projection)
    {
        return await QuerySingleTag(restaurantId, tagId)
            .Select(projection)
            .SingleOrDefaultAsync();
    }

    public async Task<Tag?> GetTag(Guid restaurantId, short tagId)
    {
        var existed = await QuerySingleTag(restaurantId, tagId)
            .AnyAsync();

        if (!existed)
        {
            return null;
        }

        return GetTagStub(restaurantId, tagId);
    }

    public async Task DeleteTag(Tag tag)
    {
        _ctx.Remove(tag);
    }
}