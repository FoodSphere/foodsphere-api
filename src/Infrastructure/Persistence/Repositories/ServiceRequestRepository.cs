namespace FoodSphere.Infrastructure.Repository;

public class ServiceRequestRepository(
    FoodSphereDbContext context,
    IPublishEndpoint publishEndpoint
) : RepositoryBase(context)
{
    public async Task<ResultObject<ServiceRequest>> CreateServiceRequest(
        BillKey billKey,
        string reason,
        CancellationToken ct = default)
    {
        var bill = await _ctx.FindAsync<Bill>(billKey, ct);

        if (bill is null)
            return ResultObject.NotFound(billKey);

        var serviceRequest = new ServiceRequest
        {
            Id = Guid.CreateVersion7(),
            BillId = billKey.Id,
            Reason = reason,
        };

        await publishEndpoint.Publish(
            new ServiceRequestCreatedMessage
            {
                Resource = serviceRequest,
                Table = new(bill.RestaurantId, bill.BranchId, bill.TableId),
            },
            ct);

        _ctx.Add(serviceRequest);

        return serviceRequest;
    }

    public IQueryable<ServiceRequest> QueryServiceRequests()
    {
        return _ctx.Set<ServiceRequest>()
            .AsExpandable();
    }

    public IQueryable<ServiceRequest> QuerySingleServiceRequest(ServiceRequestKey key)
    {
        return QueryServiceRequests()
            .Where(e =>
                e.BillId == key.BillId &&
                e.Id == key.Id);
    }

    public async Task<ServiceRequest?> GetServiceRequest(
        ServiceRequestKey key, CancellationToken ct = default)
    {
        return await _ctx.FindAsync<ServiceRequest>(key, ct);
    }
}