namespace FoodSphere.Common.Service;

public class ConsumerServiceBase(
    PersistenceService persistenceService,
    ConsumerRepository consumerRepository
) : ServiceBase
{
    public async Task<TDto[]> ListConsumers<TDto>(
        Expression<Func<ConsumerUser, TDto>> projection,
        Expression<Func<ConsumerUser, bool>> predicate,
        CancellationToken ct = default)
    {
        return await consumerRepository.QueryConsumers()
            .Where(predicate)
            .Select(projection)
            .ToArrayAsync(ct);
    }

    public async Task<ResultObject<(ConsumerUserKey, TDto)>> CreateConsumer<TDto>(
        Expression<Func<ConsumerUser, TDto>> projection,
        ConsumerCreateCommand command,
        CancellationToken ct = default)
    {
        var createResult = await consumerRepository.CreateConsumer(
            email: command.Email,
            password: command.Password,
            username: command.Username,
            phoneNumber: command.PhoneNumber,
            twoFactorEnabled: command.TwoFactorEnabled,
            ct);

        if (!createResult.TryGetValue(out var consumer))
            return createResult.Errors;

        await persistenceService.Commit(ct);

        var response = await GetConsumer(projection, consumer, ct);

        if (response is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Failed to retrieve the created consumer.");

        return (consumer, response);
    }

    public async Task<TDto?> GetConsumer<TDto>(
        Expression<Func<ConsumerUser, TDto>> projection, ConsumerUserKey key,
        CancellationToken ct = default)
    {
        return await consumerRepository.QuerySingleConsumer(key)
            .Select(projection)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<ResultObject> DeleteConsumer(
        ConsumerUserKey key, CancellationToken ct = default)
    {
        // delete image, change email to null, update username to random value, set delete time
        throw new NotImplementedException();
    }
}