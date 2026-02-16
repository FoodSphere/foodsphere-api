namespace FoodSphere.Pos.Api.Controller;

[Route("s/restaurants/{restaurant_id}/tables")]
public class SingleTableController(
    ILogger<SingleTableController> logger,
    BranchService branchService
) : PosControllerBase
{
    /// <summary>
    /// list tables
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ICollection<TableResponse>>> ListTables(Guid restaurant_id)
    {
        var responses = await branchService.QueryTables()
            .Where(e =>
                e.RestaurantId == restaurant_id &&
                e.BranchId == 1)
            .Select(TableResponse.Projection)
            .ToArrayAsync();

        return responses;
    }

    /// <summary>
    /// create table
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TableResponse>> CreateTable(Guid restaurant_id, TableRequest body)
    {
        var branch = branchService.GetBranchStub(restaurant_id, 1);

        var table = await branchService.CreateTable(
            branch: branch,
            name: body.name
        );

        await branchService.SaveChanges();

        var response = await branchService.GetTable(
            restaurant_id, 1, table.Id,
            TableResponse.Projection);

        return CreatedAtAction(
            nameof(GetTable),
            new { restaurant_id, table_id = table.Id },
            response);
    }

    /// <summary>
    /// get table
    /// </summary>
    [HttpGet("{table_id}")]
    public async Task<ActionResult<TableResponse>> GetTable(Guid restaurant_id, short table_id)
    {
        var response = await branchService.GetTable(
            restaurant_id, 1, table_id,
            TableResponse.Projection);

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }

    /// <summary>
    /// delete table
    /// </summary>
    [HttpDelete("{table_id}")]
    public async Task<ActionResult> DeleteTable(Guid restaurant_id, short table_id)
    {
        var table = await branchService.GetTable(restaurant_id, 1, table_id);

        if (table is null)
        {
            return NotFound();
        }

        await branchService.DeleteTable(table);
        await branchService.SaveChanges();

        return NoContent();
    }
}