namespace FoodSphere.SelfOrdering.Api.Controller;

[Route("requests")]
public class RequestController(
    ILogger<RequestController> logger,
    ServiceRequestService requestService
) : SelfOrderingControllerBase
{
    /// <summary>
    /// list service-request
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ICollection<ServiceRequestResponse>>> ListServiceRequest(
        [FromQuery] IReadOnlyCollection<ServiceRequestStatus> status)
    {
        Expression<Func<ServiceRequest, bool>> predicate = e =>
            e.Bill.RestaurantId == RestaurantId;

        if (status.Count > 0)
            predicate = predicate.And(e => status.Contains(e.Status));

        return await requestService.ListRequests(
            ServiceRequestResponse.Projection, predicate);;
    }

    /// <summary>
    /// create service-request
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ServiceRequestResponse>> CreateServiceRequest(
        ServiceRequestRequest body)
    {
        var result = await requestService.CreateRequest(
            ServiceRequestResponse.Projection, new(
                new(MemberKey.BillId),
                null,
                body.reason));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        var (key, response) = result.Value;

        return CreatedAtAction(
            nameof(GetServiceRequest),
            new { request_id = key.Id },
            response);
    }

    /// <summary>
    /// get service-request
    /// </summary>
    [HttpGet("{request_id}")]
    public async Task<ActionResult<ServiceRequestResponse>> GetServiceRequest(
        Guid request_id)
    {
        var response = await requestService.GetRequest(
            ServiceRequestResponse.Projection,
            new(MemberKey.BillId, request_id));

        if (response is null)
            return NotFound();

        return response;
    }

    /// <summary>
    /// cancel service-request
    /// </summary>
    [HttpDelete("{request_id}")]
    public async Task<IActionResult> CancelServiceRequest(Guid request_id)
    {
        var result = await requestService.UpdateRequestStatus(
            new(MemberKey.BillId, request_id), ServiceRequestStatus.Cancelled);

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }
}