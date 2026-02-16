using Microsoft.EntityFrameworkCore.Storage;

namespace FoodSphere.Common.Service;

public abstract class ServiceBase(FoodSphereDbContext context)
{
    protected readonly FoodSphereDbContext _ctx = context;

    public async Task<IDbContextTransaction> GetTransaction()
    {
        return await _ctx.Database.BeginTransactionAsync();
    }

    public async Task<IDbContextTransaction?> UseTransaction(IDbContextTransaction transaction)
    {
        return await _ctx.Database.UseTransactionAsync(transaction.GetDbTransaction());
    }

    public async Task<int> SaveChanges()
    {
        return await _ctx.SaveChangesAsync();
    }
}