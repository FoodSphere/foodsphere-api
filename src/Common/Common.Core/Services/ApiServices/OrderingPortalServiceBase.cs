using Bogus;

namespace FoodSphere.Common.Service;

public class OrderingPortalServiceBase(
    PersistenceService persistenceService,
    OrderingAuthService orderingAuthService,
    OrderingPortalRepository portalRepository,
    BillRepository billRepository,
    BillMemberRepository memberRepository
) : ServiceBase
{
    public async Task<ResultObject<string>> GenerateToken(
        PortalKey portalKey,
        ConsumerUserKey? consumerKey,
        string? name,
        CancellationToken ct = default)
    {
        var portal = await portalRepository.GetPortal(portalKey, ct);

        if (portal is null)
            return ResultObject.NotFound(portalKey);

        if (portal.IsUnusable)
            return ResultObject.Fail(ResultError.State,
                "Portal can not be used.");

        portal.Use();

        if (consumerKey is null)
        {
            var faker = new Faker();
            name = faker.Name.FirstName();
        }

        var result = await memberRepository.CreateMember(
            billKey: new(portal.BillId),
            name: name,
            consumerKey: consumerKey,
            ct);

        if (result.IsFailed)
            return result.Errors;

        var member = result.Value;

        var token = await orderingAuthService.GenerateToken(member);

        await persistenceService.Commit(ct);

        return token;
    }

    public async Task<TDto[]> ListPortals<TDto>(
        Expression<Func<OrderingPortal, TDto>> projection,
        Expression<Func<OrderingPortal, bool>>? predicate = null,
        CancellationToken ct = default)
    {
        var query = portalRepository.QueryPortals();

        if (predicate is not null)
            query = query.Where(predicate);

        return await query
            .Select(projection)
            .ToArrayAsync(ct);
    }

    public async Task<ResultObject<(PortalKey, TDto)>> CreatePortal<TDto>(
        Expression<Func<OrderingPortal, TDto>> projection,
        OrderingPortalCreateCommand command,
        CancellationToken ct = default)
    {
        var result = await portalRepository.CreatePortal(
            billKey: command.BillKey,
            maxUsage: command.MaxUsage,
            validDuration: command.ValidDuration,
            ct);

        if (!result.TryGetValue(out var portal))
            return result.Errors;

        await persistenceService.Commit(ct);

        var response = projection.Invoke(portal);

        return (portal, response);
    }

    public async Task<TDto?> GetPortal<TDto>(
        Expression<Func<OrderingPortal, TDto>> projection, PortalKey key,
        CancellationToken ct = default)
    {
        return await portalRepository.QuerySinglePortal(key)
            .Select(projection)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<ResultObject> UpdatePortal(
        PortalKey key, PortalUpdateCommand command,
        CancellationToken ct = default)
    {
        var portal = await portalRepository.GetPortal(key, ct);

        if (portal is null)
            return ResultObject.NotFound(key);

        portal.MaxUsage = command.MaxUsage;
        portal.ValidDuration = command.ValidDuration;

        await persistenceService.Commit();

        return ResultObject.Success();
    }
}