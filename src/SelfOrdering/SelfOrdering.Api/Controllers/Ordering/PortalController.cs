namespace FoodSphere.SelfOrdering.Api.Controller;

[Route("portals")]
public class PortalController(
    ILogger<PortalController> logger,
    OrderingPortalService orderingPortalService
) : SelfOrderingControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ICollection<PortalResponse>>> ListPortals()
    {
        var responses = await orderingPortalService.QueryPortals()
            .Where(p => p.BillId == Member.BillId)
            .Select(PortalResponse.Projection)
            .ToArrayAsync();

        return responses;
    }

    [HttpPost]
    public async Task<ActionResult<PortalResponse>> CreatePortal(PortalRequest body)
    {
        var portal = await orderingPortalService.CreatePortal(
            Member.BillId,
            body.max_usage,
            body.valid_duration
        );

        await orderingPortalService.SaveChanges();

        var response = PortalResponse.Project(portal);

        return CreatedAtAction(
            nameof(ListPortals),
            new { portal_id = portal.Id },
            response);
    }

    [HttpGet("{portal_id}")]
    public async Task<ActionResult<PortalResponse>> GetPortal(Guid portal_id)
    {
        var response = await orderingPortalService.QuerySinglePortal(portal_id)
            .Where(p => p.BillId == Member.BillId)
            .Select(PortalResponse.Projection)
            .SingleOrDefaultAsync();

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }
}