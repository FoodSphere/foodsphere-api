namespace FoodSphere.Pos.Api.Controller;

[Route("s/restaurants/{restaurant_id}/tables")]
public class SingleTableController(
    ILogger<SingleTableController> logger,
    TableServiceBase tableService,
    AccessControlService accessControl
) : PosControllerBase
{
    /// <summary>
    /// list tables
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ICollection<SingleTableResponse>>> ListTables(
        Guid restaurant_id,
        [FromQuery] bool? is_deleted = false)
    {
        Expression<Func<Table, bool>> predicate = e =>
            e.RestaurantId == restaurant_id &&
            e.BranchId == 1;

        if (is_deleted is not null)
            predicate = predicate.And(e => e.DeleteTime != null == is_deleted.Value);

        return await tableService.ListTables(
            SingleTableResponse.Projection, predicate);
    }

    /// <summary>
    /// create table
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SingleTableResponse>> CreateTable(
        Guid restaurant_id, TableRequest body)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Table.CREATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var result = await tableService.CreateTable(
            SingleTableResponse.Projection, new(
                new(restaurant_id, 1),
                body.name));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        var (tableKey, response) = result.Value;

        return CreatedAtAction(
            nameof(GetTable),
            new { restaurant_id, table_id = tableKey.Id },
            response);
    }

    /// <summary>
    /// get table
    /// </summary>
    [HttpGet("{table_id}")]
    public async Task<ActionResult<SingleTableResponse>> GetTable(
        Guid restaurant_id, short table_id)
    {
        var response = await tableService.GetTable(
            SingleTableResponse.Projection,
            new(restaurant_id, 1, table_id));

        if (response is null)
            return NotFound();

        return response;
    }

    /// <summary>
    /// update table
    /// </summary>
    [HttpPut("{table_id}")]
    public async Task<ActionResult<SingleTableResponse>> UpdateTable(
        Guid restaurant_id, short table_id,
        TableRequest body)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Table.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var result = await tableService.UpdateTable(
            new(restaurant_id, 1, table_id),
            new(body.name));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }

    /// <summary>
    /// delete table
    /// </summary>
    [HttpDelete("{table_id}")]
    public async Task<ActionResult> DeleteTable(
        Guid restaurant_id, short table_id)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Table.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        var result = await tableService.DeleteTable(
            new(restaurant_id, 1, table_id));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }

    /// <summary>
    /// get table's current bill
    /// </summary>
    [HttpGet("{table_id}/bill")]
    public async Task<ActionResult<BillResponse>> GetTableBill(
        Guid restaurant_id, short table_id)
    {
        var response = await tableService.GetBill(
            BillResponse.Projection,
            new(restaurant_id, 1, table_id));

        if (response is null)
            return NotFound();

        return response;
    }
}