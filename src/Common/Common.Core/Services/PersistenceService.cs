using Microsoft.EntityFrameworkCore.Storage;

namespace FoodSphere.Common.Service;

public class PersistenceService(FoodSphereDbContext context) : ServiceBase
{
    public Task<IDbContextTransaction> BeginTransaction(
        CancellationToken ct = default)
    {
        return context.Database.BeginTransactionAsync(ct);
    }

    public Task<IDbContextTransaction?> UseTransaction(
        IDbContextTransaction transaction,
        CancellationToken ct = default)
    {
        return context.Database.UseTransactionAsync(transaction.GetDbTransaction(), ct);
    }

    public Task<int> Commit(
        CancellationToken ct = default)
    {
        return context.SaveChangesAsync(ct);
    }
}