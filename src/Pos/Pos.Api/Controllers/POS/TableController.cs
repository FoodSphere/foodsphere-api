namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}/branches/{branch_id}/tables")]
public class TableController(
    ILogger<TableController> logger,
    BranchService branchService
) : PosControllerBase
{
    /// <summary>
    /// list tables
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ICollection<TableResponse>>> ListTables(Guid restaurant_id, short branch_id)
    {
        var responses = await branchService.QueryTables()
            .Where(e =>
                e.RestaurantId == restaurant_id &&
                e.BranchId == branch_id)
            .Select(TableResponse.Projection)
            .ToArrayAsync();

        return responses;
    }

    /// <summary>
    /// create table
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TableResponse>> CreateTable(Guid restaurant_id, short branch_id, TableRequest body)
    {
        var branch = branchService.GetBranchStub(restaurant_id, branch_id);

        var table = await branchService.CreateTable(
            branch: branch,
            name: body.name
        );

        await branchService.SaveChanges();

        var response = await branchService.GetTable(
            restaurant_id, branch_id, table.Id,
            TableResponse.Projection
        );

        return CreatedAtAction(
            nameof(GetTable),
            new { restaurant_id, branch_id, table_id = table.Id },
            response);
    }

    /// <summary>
    /// get table
    /// </summary>
    [HttpGet("{table_id}")]
    public async Task<ActionResult<TableResponse>> GetTable(Guid restaurant_id, short branch_id, short table_id)
    {
        var response = await branchService.GetTable(
            restaurant_id, branch_id, table_id,
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
    public async Task<ActionResult> DeleteTable(Guid restaurant_id, short branch_id, short table_id)
    {
        var table = await branchService.GetTable(restaurant_id, branch_id, table_id);

        if (table is null)
        {
            return NotFound();
        }

        await branchService.DeleteTable(table);
        await branchService.SaveChanges();

        return NoContent();
    }
}