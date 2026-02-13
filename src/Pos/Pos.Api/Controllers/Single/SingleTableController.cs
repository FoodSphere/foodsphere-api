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
    public async Task<ActionResult<List<TableResponse>>> ListTables(Guid restaurant_id)
    {
        var tables = await branchService.ListDefaultTables(restaurant_id);

        return tables
            .Select(TableResponse.FromModel)
            .ToList();
    }

    /// <summary>
    /// create table
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TableResponse>> CreateTable(Guid restaurant_id, TableRequest body)
    {
        var branch = await branchService.GetDefaultBranch(restaurant_id);

        if (branch is null)
        {
            return NotFound();
        }

        var table = await branchService.CreateTable(
            branch: branch,
            name: body.name
        );

        await branchService.SaveChanges();

        return CreatedAtAction(
            nameof(GetTable),
            new { restaurant_id, table_id = table.Id },
            TableResponse.FromModel(table)
        );
    }

    /// <summary>
    /// get table
    /// </summary>
    [HttpGet("{table_id}")]
    public async Task<ActionResult<TableResponse>> GetTable(Guid restaurant_id, short table_id)
    {
        var table = await branchService.GetDefaultTable(restaurant_id, table_id);

        if (table is null)
        {
            return NotFound();
        }

        return TableResponse.FromModel(table);
    }

    /// <summary>
    /// delete table
    /// </summary>
    [HttpDelete("{table_id}")]
    public async Task<ActionResult> DeleteTable(Guid restaurant_id, short table_id)
    {
        var table = await branchService.GetDefaultTable(restaurant_id, table_id);

        if (table is null)
        {
            return NotFound();
        }

        await branchService.DeleteTable(table);
        await branchService.SaveChanges();

        return NoContent();
    }
}