using MassTransit.Initializers;

namespace FoodSphere.Common.Service;

public class OrderServiceBase(
    PersistenceService persistenceService,
    IPublishEndpoint publishEndpoint,
    MenuRepository menuRepository,
    OrderRepository orderRepository,
    BillRepository billRepository
) : ServiceBase
{
    async Task<ResultObject> PrefetchMenus(
        IEnumerable<MenuKey> menuKeys,
        CancellationToken ct = default)
    {
        var restaurantIds = menuKeys
            .Select(i => i.RestaurantId)
            .Distinct()
            .ToArray();

        if (restaurantIds.Length == 0)
            return ResultObject.Success();

        if (restaurantIds.Length > 1)
            return ResultObject.Fail(ResultError.Argument,
                "All menu keys must belong to the same restaurant.",
                restaurantIds);

        var menuIds = menuKeys.Select(i => i.Id)
            .ToHashSet();

        var queriedMenu = await menuRepository.QueryMenus()
            .Where(e =>
                e.RestaurantId == restaurantIds[0] &&
                menuIds.Contains(e.Id))
            .ToArrayAsync(ct);

        var missingIds = menuIds.Except(
            queriedMenu.Select(e => e.Id));

        if (missingIds.Any())
            return ResultObject.Fail(ResultError.NotFound,
                "Menus not found", new { menu_ids = missingIds });

        return ResultObject.Success();
    }

    async Task<ResultObject> CreateOrderItems(
        OrderKey key, IEnumerable<OrderItemCreateCommand> command,
        CancellationToken ct = default)
    {
        foreach (var item in command)
        {
            var result = await orderRepository.CreateItem(key,
                item.MenuKey,
                item.Quantity,
                item.Note,
                ct);

            if (result.IsFailed)
                return result.Errors;
        }

        return ResultObject.Success();
    }

    public async Task<TDto[]> ListOrders<TDto>(
        Expression<Func<Order, TDto>> projection,
        Expression<Func<Order, bool>> predicate,
        CancellationToken ct = default)
    {
        return await orderRepository.QueryOrders()
            .Where(predicate)
            .Select(projection)
            .ToArrayAsync(ct);
    }

    public async Task<ResultObject<(OrderKey, TDto)>> CreateOrder<TDto>(
        Expression<Func<Order, TDto>> projection,
        BillKey billKey,
        OrderCreateCommand command,
        CancellationToken ct = default)
    {
        var createResult = await orderRepository.CreateOrder(billKey, ct);

        if (!createResult.TryGetValue(out var order))
            return createResult.Errors;

        order.Status = command.Status;

        var prefetchResult = await PrefetchMenus(
            command.Items.Select(i => i.MenuKey), ct);

        if (prefetchResult.IsFailed)
            return prefetchResult.Errors;

        var itemResult = await CreateOrderItems(order, command.Items, ct);

        if (itemResult.IsFailed)
            return itemResult.Errors;

        await persistenceService.Commit(ct);

        var response = await GetOrder(projection, order, ct);

        if (response is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Failed to retrieve the created order.");

        return (order, response);
    }

    public async Task<ResultObject<IEnumerable<(OrderKey, TDto)>>> CreateBulkOrders<TDto>(
        Expression<Func<Order, TDto>> projection,
        BillKey billKey,
        IEnumerable<OrderCreateCommand> commands,
        CancellationToken ct = default)
    {
        var orderIds = new List<short>();

        var prefetchResult = await PrefetchMenus(
            commands
                .SelectMany(i => i.Items)
                .Select(i => i.MenuKey), ct);

        if (prefetchResult.IsFailed)
            return prefetchResult.Errors;

        foreach (var cmd in commands)
        {
            var createResult = await orderRepository.CreateOrder(billKey, ct);

            if (!createResult.TryGetValue(out var order))
                return createResult.Errors;

            order.Status = cmd.Status;
            orderIds.Add(order.Id);

            var itemResult = await CreateOrderItems(order, cmd.Items, ct);

            if (itemResult.IsFailed)
                return itemResult.Errors;
        }

        await persistenceService.Commit(ct);

        var responses = await orderRepository.QueryOrders(billKey)
            .Where(e => orderIds.Contains(e.Id))
            .Select(
                e => new
                {
                    OrderKey = new OrderKey(e.BillId, e.Id),
                    Response = projection.Invoke(e)
                })
            .ToArrayAsync(ct);

        var result = responses.Select(i =>
            (i.OrderKey, i.Response));

        return ResultObject.Success(result);
    }

    public async Task<TDto?> GetOrder<TDto>(
        Expression<Func<Order, TDto>> projection, OrderKey key,
        CancellationToken ct = default)
    {
        return await orderRepository.QuerySingleOrder(key)
            .Select(projection)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<ResultObject> UpdateOrderStatus(
        OrderKey key, OrderStatus status)
    {
        var result = await orderRepository.UpdateOrderStatus(key, status);

        if (result.IsFailed)
            return result.Errors;

        await persistenceService.Commit();

        return ResultObject.Success();
    }

    public async Task<ResultObject> SetOrderItems(
        OrderKey key, IEnumerable<OrderItemCreateCommand> command,
        CancellationToken ct = default)
    {
        await using var transaction = await persistenceService.BeginTransaction(ct);

        await orderRepository
            .QueryItems(key)
            .ExecuteDeleteAsync(ct);

        var prefetchResult = await PrefetchMenus(
            command.Select(i => i.MenuKey), ct);

        if (prefetchResult.IsFailed)
            return prefetchResult.Errors;

        await CreateOrderItems(key, command, ct);

        await persistenceService.Commit(ct);
        await transaction.CommitAsync(ct);

        return ResultObject.Success();
    }

    public async Task<ResultObject> DeleteOrder(
        OrderKey key, CancellationToken ct = default)
    {
        var result = await orderRepository.DeleteOrder(key, ct);

        if (result.IsFailed)
            return result.Errors;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }

    public async Task<TDto[]> ListItems<TDto>(
        Expression<Func<OrderItem, TDto>> projection,
        Expression<Func<OrderItem, bool>> predicate,
        CancellationToken ct = default)
    {
        return await orderRepository.QueryItems()
            .Where(predicate)
            .Select(projection)
            .ToArrayAsync(ct);
    }

    public async Task<ResultObject<(OrderItem, TDto)>> CreateItem<TDto>(
        Expression<Func<OrderItem, TDto>> projection,
        OrderKey orderKey, MenuKey menuKey,
        short quantity,
        string? note,
        CancellationToken ct = default)
    {
        var result = await orderRepository.CreateItem(
            orderKey, menuKey, quantity, note, ct);

        if (!result.TryGetValue(out var item))
            return result.Errors;

        await persistenceService.Commit(ct);

        var response = await GetItem(projection,
            new(item.BillId, item.OrderId, item.MenuId),
            ct);

        if (response is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Failed to retrieve the created order item.");

        return (item, response);
    }

    public async Task<TDto?> GetItem<TDto>(
        Expression<Func<OrderItem, TDto>> projection, OrderItemKey key,
        CancellationToken ct = default)
    {
        return await orderRepository.QuerySingleItem(key)
            .Select(projection)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<ResultObject> UpdateItem(
        OrderItemKey key, OrderItemUpdateCommand command,
        CancellationToken ct = default)
    {
        var billKey = new BillKey(key.BillId);

        var bill = await billRepository.GetBill(billKey, ct);

        if (bill is null)
            return ResultObject.NotFound(billKey);

        if (bill.Status is not BillStatus.Open)
            return ResultObject.Fail(ResultError.State,
                "only open bills can do.");

        var item = await orderRepository.GetItem(key, ct);

        if (item is null)
            return ResultObject.NotFound(key);

        item.Quantity = command.Quantity;
        item.Note = command.Note;

        await publishEndpoint.Publish(
            new OrderItemUpdatedMessage
            {
                Resource = key,
            },
            ct);

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }

    public async Task<ResultObject> DeleteItem(
        OrderItemKey key, CancellationToken ct = default)
    {
        var result = await orderRepository.DeleteItem(key);

        if (result.IsFailed)
            return result.Errors;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }
}