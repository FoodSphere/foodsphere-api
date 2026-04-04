namespace FoodSphere.SelfOrdering.Api.Controller;

[Route("portals")]
public class PortalController(
    ILogger<PortalController> logger,
    OrderingPortalServiceBase orderingPortalService
) : SelfOrderingControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ICollection<PortalResponse>>> ListPortals()
    {
        return await orderingPortalService.ListPortals(
            PortalResponse.Projection, e =>
                e.BillId == MemberKey.BillId);
    }

    [HttpPost]
    public async Task<ActionResult<PortalResponse>> CreatePortal(
        PortalRequest body)
    {
        var result = await orderingPortalService.CreatePortal(
            PortalResponse.Projection, new(
                new(MemberKey.BillId),
                body.max_usage,
                body.valid_duration));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        var (portalKey, response) = result.Value;

        return CreatedAtAction(
            nameof(ListPortals),
            new { portal_id = portalKey.Id },
            response);
    }

    [HttpGet("{portal_id}")]
    public async Task<ActionResult<PortalResponse>> GetPortal(
        Guid portal_id)
    {
        var response = await orderingPortalService.GetPortal(
            e => new
            {
                Response = PortalResponse.Projection.Invoke(e),
                BillKey = new BillKey(e.BillId)
            },
            new(portal_id));

        if (response is null)
            return NotFound();

        if (response.BillKey.Id != MemberKey.BillId)
            return NotFound();

        return response.Response;
    }

    /// <summary>
    /// update ordering's portal
    /// </summary>
    [HttpPut("{portal_id}")]
    public async Task<ActionResult> UpdatePortal(
        Guid portal_id, PortalResponse body)
    {
        var billKey = await orderingPortalService.GetPortal(
            e => new BillKey(e.BillId),
            new(portal_id));

        if (billKey is null)
            return NotFound();

        if (billKey.Id != MemberKey.BillId)
            return NotFound();

        var updateResult = await orderingPortalService.UpdatePortal(
            new(portal_id), new(
                body.max_usage,
                body.valid_duration));

        if (updateResult.IsFailed)
            return updateResult.Errors.ToActionResult();

        return NoContent();
    }
}