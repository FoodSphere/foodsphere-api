namespace FoodSphere.Pos.Api.Controller;

[Route("s/restaurants/{restaurant_id}/bills")]
public class SingleBillController(
    ILogger<BillController> logger,
    BillServiceBase billService,
    OrderingPortalServiceBase orderingPortalService
) : PosControllerBase
{
    /// <summary>
    /// list bills in main branch
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ICollection<BillResponse>>> ListBills(
        Guid restaurant_id,
        [FromQuery] IReadOnlyCollection<BillStatus> status)
    {
        Expression<Func<Bill, bool>> predicate = e =>
            e.RestaurantId == restaurant_id &&
            e.BranchId == 1;

        if (status.Count != 0)
            predicate = predicate.And(e => status.Contains(e.Status));

        return await billService.ListBills(
            BillResponse.Projection, predicate);
    }

    /// <summary>
    /// create bill
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<BillResponse>> CreateBill(
        Guid restaurant_id, BillRequest body)
    {
        var result = await billService.CreateBill(
            BillResponse.Projection, new(
                new(restaurant_id, 1, body.table_id),
                body.consumer_id is Guid consumerId ? new(consumerId) : null,
                body.pax));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        var (key, response) = result.Value;

        return CreatedAtAction(
            nameof(GetBill),
            new { restaurant_id, bill_id = key.Id },
            response);
    }

    /// <summary>
    /// get bill
    /// </summary>
    [HttpGet("{bill_id}")]
    public async Task<ActionResult<BillResponse>> GetBill(
        Guid restaurant_id, Guid bill_id)
    {
        var response = await billService.GetBill(
            BillResponse.Projection, new(bill_id));

        if (response is null)
            return NotFound();

        return response;
    }

    /// <summary>
    /// update bill
    /// </summary>
    [HttpPut("{bill_id}")]
    public async Task<ActionResult> UpdateBill(
        Guid restaurant_id, Guid bill_id, BillRequest body)
    {
        var result = await billService.UpdateBill(
            new(bill_id),
            new(
                body.table_id,
                body.consumer_id,
                body.pax));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }

    /// <summary>
    /// complete bill
    /// </summary>
    [HttpPut("{bill_id}/complete")]
    public async Task<ActionResult> CompleteBill(
        Guid restaurant_id, Guid bill_id)
    {
        var result = await billService.CompleteBill(
            new(bill_id));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }

    /// <summary>
    /// cancel bill
    /// </summary>
    [HttpDelete("{bill_id}")]
    public async Task<ActionResult> CancelBill(
        Guid restaurant_id, Guid bill_id)
    {
        var result = await billService.CancelBill(
            new(bill_id));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }

    /// <summary>
    /// list ordering's portals
    /// </summary>
    [HttpGet("{bill_id}/portals")]
    public async Task<ActionResult<ICollection<OrderingPortalResponse>>> ListPortals(
        Guid restaurant_id, Guid bill_id)
    {
        return await orderingPortalService.ListPortals(
            OrderingPortalResponse.Projection, e =>
                e.BillId == bill_id &&
                e.Bill.RestaurantId == restaurant_id &&
                e.Bill.BranchId == 1);
    }

    /// <summary>
    /// create ordering's portal
    /// </summary>
    [HttpPost("{bill_id}/portals")]
    public async Task<ActionResult<OrderingPortalResponse>> CreatePortal(
        Guid restaurant_id,
        Guid bill_id, OrderingPortalRequest body)
    {
        var result = await orderingPortalService.CreatePortal(
            OrderingPortalResponse.Projection, new(
                new(bill_id),
                body.max_usage,
                body.valid_duration));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        var (key, response) = result.Value;

        return CreatedAtAction(
            nameof(GetPortal),
            new { restaurant_id, bill_id, portal_id = key.Id },
            response);
    }

    /// <summary>
    /// get ordering's portal
    /// </summary>
    [HttpGet("{bill_id}/portals/{portal_id}")]
    public async Task<ActionResult<OrderingPortalResponse>> GetPortal(
        Guid restaurant_id, Guid bill_id, Guid portal_id)
    {
        var response = await orderingPortalService.GetPortal(
            OrderingPortalResponse.Projection,
            new(portal_id));

        if (response is null)
            return NotFound();

        return response;
    }

    /// <summary>
    /// update ordering's portal
    /// </summary>
    [HttpPut("{bill_id}/portals/{portal_id}")]
    public async Task<ActionResult> UpdatePortal(
        Guid restaurant_id, Guid bill_id, Guid portal_id,
        OrderingPortalRequest body)
    {
        var result = await orderingPortalService.UpdatePortal(
            new(portal_id),
            new(
                body.max_usage,
                body.valid_duration));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }
}