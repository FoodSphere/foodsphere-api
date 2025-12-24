using FoodSphere.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodSphere.Services;

public class OrderingService(
    AppDbContext context,
    OrderingAuthService orderingAuthService,
    BillService billService
) : BaseService(context)
{
    readonly OrderingAuthService _orderingAuthService = orderingAuthService;
    readonly BillService _billService = billService;

    public async Task<SelfOrderingPortal> CreatePortal(
        Guid billId,
        short? maxUsage = null,
        TimeSpan? validDuration = null
        // Guid? issuerId = null
    ) {
        var portal = new SelfOrderingPortal
        {
            BillId = billId,
            MaxUsage = maxUsage,
            // IssuerId = issuerId,
            ValidDuration = validDuration
        };

        await _ctx.AddAsync(portal);

        return portal;
    }

    public async Task<SelfOrderingPortal?> GetPortal(Guid portal_id)
    {
        return await _ctx.FindAsync<SelfOrderingPortal>(portal_id);
    }

    public async Task<List<SelfOrderingPortal>> ListPortals(Guid billId)
    {
        return await _ctx.Set<SelfOrderingPortal>()
            .Where(portal => portal.BillId == billId)
            .ToListAsync();
    }

    public async Task<List<SelfOrderingPortal>> ListPortals(Bill bill)
    {
        return await _ctx.Set<SelfOrderingPortal>()
            .Where(portal => portal.BillId == bill.Id)
            .ToListAsync();
    }

    public async Task<string> GenerateToken(
        SelfOrderingPortal portal,
        Guid? consumerId = null
    ) {
        if (!portal.IsValid())
        {
            throw new Exception("Ordering link invalid.");
        }

        portal.Use();
        await Save();

        var billMember = await _billService.AddBillMember(portal.BillId, consumerId);
        var token = await _orderingAuthService.GenerateToken(billMember);

        return token;
    }
}