using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FoodSphere.Services;
using FoodSphere.Data.Models;
using FoodSphere.Data;
using FoodSphere.Data.DTOs;

namespace FoodSphere.Controllers.Client;

public class TableRequest
{
    public string? name { get; set; }
}

public class TableResponse
{
    public int id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid restaurant_id { get; set; }
    public short branch_id { get; set; }

    public string? name { get; set; }
    public TableStatus status { get; set; }

    public static TableResponse FromModel(Table model)
    {
        return new TableResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            restaurant_id = model.RestaurantId,
            branch_id = model.BranchId,
            name = model.Name,
            status = model.Status,
        };
    }
}

[Route("client/restaurants/{restaurant_id}/branches/{branch_id}/tables")]
public class TableController(
    ILogger<TableController> logger,
    BranchService branchService
) : ClientController
{
    readonly ILogger<TableController> _logger = logger;
    readonly BranchService _branchService = branchService;

    [HttpPost]
    public async Task<ActionResult<TableResponse>> CreateTable(Guid restaurant_id, short branch_id, TableRequest body)
    {
        var branch = await _branchService.GetBranch(restaurant_id, branch_id);

        if (branch is null)
        {
            return NotFound();
        }

        var table = await _branchService.CreateTable(
            branch: branch,
            name: body.name
        );
        await _branchService.Save();

        return CreatedAtAction(
            nameof(GetTable),
            new { restaurant_id, branch_id, table_id = table.Id },
            TableResponse.FromModel(table)
        );
    }

    [HttpGet]
    public async Task<ActionResult<List<TableResponse>>> ListTables(Guid restaurant_id, short branch_id)
    {
        var tables = await _branchService.ListTables(restaurant_id, branch_id);

        return tables
            .Select(TableResponse.FromModel)
            .ToList();
    }

    [HttpGet("{table_id}")]
    public async Task<ActionResult<TableResponse>> GetTable(Guid restaurant_id, short branch_id, short table_id)
    {
        var table = await _branchService.GetTable(restaurant_id, branch_id, table_id);

        if (table is null)
        {
            return NotFound();
        }

        return TableResponse.FromModel(table);
    }

    [HttpDelete("{table_id}")]
    public async Task<ActionResult> DeleteTable(Guid restaurant_id, short branch_id, short table_id)
    {
        var table = await _branchService.GetTable(restaurant_id, branch_id, table_id);

        if (table is null)
        {
            return NotFound();
        }

        await _branchService.DeleteTable(table);
        await _branchService.Save();

        return NoContent();
    }
}