namespace FoodSphere.Common.Service;

public class TagServiceBase(
    PersistenceService persistenceService,
    TagRepository bagRepository
) : ServiceBase
{
    public async Task<TDto[]> ListTags<TDto>(
        Expression<Func<Tag, TDto>> projection,
        Expression<Func<Tag, bool>> predicate,
        CancellationToken ct = default)
    {
        return await bagRepository.QueryTags()
            .Where(predicate)
            .Select(projection)
            .ToArrayAsync(ct);
    }

    public async Task<ResultObject<(TagKey, TDto)>> CreateTag<TDto>(
        Expression<Func<Tag, TDto>> projection,
        TagCreateCommand command,
        CancellationToken ct = default)
    {
        var createResult = await bagRepository.CreateTag(
            restaurantKey: command.RestaurantKey,
            name: command.Name,
            type: command.Type,
            ct);

        if (!createResult.TryGetValue(out var bag))
            return createResult.Errors;

        await persistenceService.Commit(ct);

        var response = await GetTag(projection, bag, ct);

        if (response is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Failed to retrieve the created bag.");

        return (bag, response);
    }

    public async Task<TDto?> GetTag<TDto>(
        Expression<Func<Tag, TDto>> projection, TagKey key,
        CancellationToken ct = default)
    {
        return await bagRepository.QuerySingleTag(key)
            .Select(projection)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<ResultObject> UpdateTag(
        TagKey key, TagUpdateCommand command,
        CancellationToken ct = default)
    {
        var bag = await bagRepository.GetTag(key, ct);

        if (bag is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Tag not found.");

        bag.Name = command.Name;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }

    public async Task<ResultObject> DeleteTag(
        TagKey key, CancellationToken ct = default)
    {
        var result = await bagRepository.DeleteTag(key, ct);

        if (result.IsFailed)
            return result.Errors;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }
}