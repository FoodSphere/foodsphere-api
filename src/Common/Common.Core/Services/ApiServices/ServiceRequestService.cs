namespace FoodSphere.Common.Service;

public class ServiceRequestService(
    PersistenceService persistenceService,
    IPublishEndpoint publishEndpoint,
    ServiceRequestRepository requestRepository
) : ServiceBase
{
    public async Task<TDto[]> ListRequests<TDto>(
        Expression<Func<ServiceRequest, TDto>> projection,
        Expression<Func<ServiceRequest, bool>> predicate,
        CancellationToken ct = default)
    {
        return await requestRepository.QueryServiceRequests()
            .Where(predicate)
            .Select(projection)
            .ToArrayAsync(ct);
    }

    public async Task<ResultObject<(ServiceRequestKey, TDto)>> CreateRequest<TDto>(
        Expression<Func<ServiceRequest, TDto>> projection,
        ServiceRequestCreateCommand command,
        CancellationToken ct = default)
    {
        var requestResult = await requestRepository.CreateServiceRequest(
            billKey: command.BillKey,
            reason: command.Reason,
            ct);

        if (!requestResult.TryGetValue(out var request))
            return requestResult.Errors;

        await persistenceService.Commit(ct);

        var response = await GetRequest(projection, request, ct);

        if (response is null)
            return ResultObject.Fail(ResultError.NotFound,
                "Failed to retrieve the created request.");

        return (request, response);
    }

    public async Task<TDto?> GetRequest<TDto>(
        Expression<Func<ServiceRequest, TDto>> projection, ServiceRequestKey key,
        CancellationToken ct = default)
    {
        return await requestRepository.QuerySingleServiceRequest(key)
            .Select(projection)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<ResultObject> UpdateRequestStatus(
        ServiceRequestKey key, ServiceRequestStatus status,
        CancellationToken ct = default)
    {
        var request = await requestRepository.GetServiceRequest(key, ct);

        if (request is null)
            return ResultObject.NotFound(key);

        request.Status = status;

        await publishEndpoint.Publish(
            new ServiceRequestStatusUpdatedMessage
            {
                Resource = key,
                Status = status
            }
            , ct);

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }
}