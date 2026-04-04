namespace FoodSphere.Common.Service;

public class TableServiceBase(
    PersistenceService persistenceService,
    TableRepository tableRepository,
    BillRepository billRepository
) : ServiceBase
{
    public async Task<TDto[]> ListTables<TDto>(
        Expression<Func<Table, TDto>> projection,
        Expression<Func<Table, bool>> predicate,
        CancellationToken ct = default)
    {
        return await tableRepository.QueryTables()
            .Where(predicate)
            .OrderBy(t => t.Id)
            .Select(projection)
            .ToArrayAsync(ct);
    }

    public async Task<ResultObject<(TableKey, TDto)>> CreateTable<TDto>(
        Expression<Func<Table, TDto>> projection,
        TableCreateCommand command,
        CancellationToken ct = default)
    {
        var createResult = await tableRepository.CreateTable(
            branchKey: command.BranchKey,
            name: command.Name,
            ct);

        if (!createResult.TryGetValue(out var table))
            return createResult.Errors;

        await persistenceService.Commit(ct);

        var response = await GetTable(projection, table, ct);

        if (response is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Failed to retrieve the created table.");

        return (table, response);
    }

    public async Task<TDto?> GetTable<TDto>(
        Expression<Func<Table, TDto>> projection, TableKey key,
        CancellationToken ct = default)
    {
        return await tableRepository.QuerySingleTable(key)
            .Select(projection)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<ResultObject> UpdateTable(
        TableKey key, TableUpdateCommand command,
        CancellationToken ct = default)
    {
        var table = await tableRepository.GetTable(key, ct);

        if (table is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Table not found.");

        table.Name = command.Name;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }

    public async Task<ResultObject> DeleteTable(
        TableKey key, CancellationToken ct = default)
    {
        var result = await tableRepository.DeleteTable(key, ct);

        if (result.IsFailed)
            return result.Errors;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }

    public async Task<TDto?> GetBill<TDto>(
        Expression<Func<Bill, TDto>> projection, TableKey key,
        CancellationToken ct = default)
    {
        return await billRepository.QueryBills()
            .Where(bill =>
                bill.RestaurantId == key.RestaurantId &&
                bill.BranchId == key.BranchId &&
                bill.TableId == key.Id &&
                bill.Status == BillStatus.Open)
            .Select(projection)
            .SingleOrDefaultAsync(ct);
    }
}