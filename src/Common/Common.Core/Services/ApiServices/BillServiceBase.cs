namespace FoodSphere.Common.Service;

public class BillServiceBase(
    PersistenceService persistenceService,
    IPublishEndpoint publishEndpoint,
    BillRepository billRepository,
    TableRepository tableRepository
) : ServiceBase
{
    public async Task<TDto[]> ListBills<TDto>(
        Expression<Func<Bill, TDto>> projection,
        Expression<Func<Bill, bool>> predicate,
        CancellationToken ct = default)
    {
        return await billRepository.QueryBills()
            .Where(predicate)
            .Select(projection)
            .ToArrayAsync(ct);
    }

    public async Task<ResultObject<(BillKey, TDto)>> CreateBill<TDto>(
        Expression<Func<Bill, TDto>> projection,
        BillCreateCommand command,
        CancellationToken ct = default)
    {
        var billResult = await billRepository.CreateBill(
            tableKey: command.TableKey,
            pax: command.Pax,
            consumerKey: command.ConsumerKey,
            ct);

        if (!billResult.TryGetValue(out var bill))
            return billResult.Errors;

        var statusResult = await tableRepository.UpdateTableStatus(
            command.TableKey, TableStatus.Occupied, ct);

        if (statusResult.IsFailed)
            return statusResult.Errors;

        await persistenceService.Commit(ct);

        var response = await GetBill(projection, bill, ct);

        if (response is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Failed to retrieve the created bill.");

        return (bill, response);
    }

    public async Task<TDto?> GetBill<TDto>(
        Expression<Func<Bill, TDto>> projection, BillKey key,
        CancellationToken ct = default)
    {
        return await billRepository.QuerySingleBill(key)
            .Select(projection)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<ResultObject> UpdateBill(
        BillKey key, BillUpdateCommand command,
        CancellationToken ct = default)
    {
        var bill = await billRepository.GetBill(key, ct);

        if (bill is null)
            return ResultObject.NotFound(key);

        bill.TableId = command.TableId;
        bill.ConsumerId = command.ConsumerId;
        bill.Pax = command.Pax;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }

    public async Task<ResultObject> CompleteBill(
        BillKey key, CancellationToken ct = default)
    {
        var bill = await billRepository.GetBill(key, ct);

        if (bill is null)
            return ResultObject.NotFound(key);

        if (bill.Status is not BillStatus.Paid)
            return ResultObject.Fail(ResultError.State,
                "Only paid bills can be completed.");

        bill.Status = BillStatus.Completed;

        await publishEndpoint.Publish(
            new BillStatusUpdatedMessage
            {
                Resource = bill,
                Status = bill.Status,
                Branch = new(bill.RestaurantId, bill.BranchId)
            }
            , ct);

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }

    public async Task<ResultObject> CancelBill(
        BillKey key, CancellationToken ct = default)
    {
        var bill = await billRepository.GetBill(key, ct);

        if (bill is null)
            return ResultObject.NotFound(key);

        if (bill.Status is BillStatus.Paid)
            return ResultObject.Fail(ResultError.State,
                "paid bills can not be cancelled.");

        bill.Status = BillStatus.Cancelled;

        await publishEndpoint.Publish(
            new BillStatusUpdatedMessage
            {
                Resource = bill,
                Status = bill.Status,
                Branch = new(bill.RestaurantId, bill.BranchId)
            }
            , ct);

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }
}