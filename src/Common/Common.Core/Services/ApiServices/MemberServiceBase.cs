namespace FoodSphere.Common.Service;

public class MemberServiceBase(
    PersistenceService persistenceService,
    BillMemberRepository memberRepository
) : ServiceBase
{
    public async Task<TDto[]> ListMembers<TDto>(
        Expression<Func<BillMember, TDto>> projection,
        Expression<Func<BillMember, bool>> predicate,
        CancellationToken ct = default)
    {
        return await memberRepository.QueryMembers()
            .Where(predicate)
            .Select(projection)
            .ToArrayAsync(ct);
    }

    public async Task<ResultObject<(BillMemberKey, TDto)>> CreateMember<TDto>(
        Expression<Func<BillMember, TDto>> projection,
        MemberCreateCommand command,
        CancellationToken ct = default)
    {
        var createResult = await memberRepository.CreateMember(
            billKey: command.BillKey,
            name: command.Name,
            consumerKey: command.ConsumerKey,
            ct);

        if (!createResult.TryGetValue(out var member))
            return createResult.Errors;

        await persistenceService.Commit(ct);

        var response = await GetMember(projection, member, ct);

        if (response is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Failed to retrieve the created member.");

        return (member, response);
    }

    public async Task<TDto?> GetMember<TDto>(
        Expression<Func<BillMember, TDto>> projection, BillMemberKey key,
        CancellationToken ct = default)
    {
        return await memberRepository.QuerySingleMember(key)
            .Select(projection)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<ResultObject> UpdateMember(
        BillMemberKey key, MemberUpdateCommand command,
        CancellationToken ct = default)
    {
        var member = await memberRepository.QuerySingleMember(key)
            .SingleOrDefaultAsync(ct);

        if (member is null)
            return ResultObject.NotFound(key);

        member.Name = command.Name;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }

    public async Task<ResultObject> SetConsumer(
        BillMemberKey key, ConsumerUserKey consumerKey,
        CancellationToken ct = default)
    {
        var member = await memberRepository.QuerySingleMember(key)
            .SingleOrDefaultAsync(ct);

        if (member is null)
            return ResultObject.NotFound(key);

        member.ConsumerId = consumerKey.Id;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }

    public async Task<ResultObject> DeleteMember(
        BillMemberKey key, CancellationToken ct = default)
    {
        var result = await memberRepository.DeleteMember(key, ct);

        if (result.IsFailed)
            return result.Errors;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }
}