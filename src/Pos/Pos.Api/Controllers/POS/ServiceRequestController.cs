namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}")]
public class ServiceRequestController(
    ILogger<ServiceRequestController> logger,
    ServiceRequestService requestService
) : PosControllerBase
{
    [HttpGet("service-requests")]
    public async Task<ActionResult<ICollection<ServiceRequestResponse>>> ListServiceRequest(
        Guid restaurant_id,
        [FromQuery] IReadOnlyCollection<Guid> bill_id,
        [FromQuery] IReadOnlyCollection<ServiceRequestStatus> status)
    {
        Expression<Func<ServiceRequest, bool>> predicate = e =>
            e.Bill.RestaurantId == restaurant_id;

        if (bill_id.Count > 0)
            predicate = predicate.And(e => bill_id.Contains(e.BillId));

        if (status.Count > 0)
            predicate = predicate.And(e => status.Contains(e.Status));

        return await requestService.ListRequests(
            ServiceRequestResponse.Projection, predicate);;
    }

    [HttpGet("bills/{bill_id}/service-requests/{request_id}")]
    public async Task<ActionResult<ServiceRequestResponse>> GetServiceRequest(
        Guid restaurant_id, Guid bill_id, Guid request_id)
    {
        var response = await requestService.GetRequest(
            ServiceRequestResponse.Projection,
            new(bill_id, request_id));

        if (response is null)
            return NotFound();

        return response;
    }

    [HttpPut("bills/{bill_id}/service-requests/{request_id}")]
    public async Task<IActionResult> UpdateServiceRequestStatus(
        Guid restaurant_id, Guid bill_id, Guid request_id,
        ServiceRequestStatusRequest body)
    {
        var result = await requestService.UpdateRequestStatus(
            new(bill_id, request_id), body.status);

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }
}