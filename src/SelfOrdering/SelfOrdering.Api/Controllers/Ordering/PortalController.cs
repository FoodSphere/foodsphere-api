using Microsoft.AspNetCore.Mvc;

namespace FoodSphere.SelfOrdering.Api.Controllers;

[Route("portals")]
public class PortalController(
    ILogger<PortalController> logger,
    OrderingPortalService orderingPortalService
) : SelfOrderingControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<PortalResponse>>> ListPortals([FromQuery] Guid? portal_id)
    {
        var portals = await orderingPortalService.ListPortals(Member.BillId);

        if (portal_id is not null)
        {
            portals = [.. portals.Where(p => p.Id == portal_id)];
        }

        return portals
            .Select(PortalResponse.FromModel)
            .ToList();
    }

    [HttpPost]
    public async Task<ActionResult<PortalResponse>> CreatePortal(PortalRequest body)
    {
        var portal = await orderingPortalService.CreatePortal(
            Member.BillId,
            body.max_usage,
            body.valid_duration
        );

        await orderingPortalService.SaveAsync();

        return CreatedAtAction(
            nameof(ListPortals),
            new { portal_id = portal.Id },
            PortalResponse.FromModel(portal)
        );
    }
}