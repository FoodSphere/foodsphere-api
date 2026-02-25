namespace FoodSphere.Resource.Api.Controller;

[Route("restaurants/{restaurant_id}/branches")]
public class BranchController(
    ILogger<BranchController> logger,
    BranchService branchService,
    StaffService staffService
) : ResourceControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<BranchResponse>>> ListBranches(Guid restaurant_id)
    {
        var branches = await branchService.ListBranches(restaurant_id);

        return branches
            .Select(BranchResponse.FromModel)
            .ToList();
    }

    [HttpPost]
    public async Task<ActionResult<BranchResponse>> CreateBranch(Guid restaurant_id, BranchRequest body)
    {
        var branch = await branchService.CreateBranch(
            restaurantId: restaurant_id,
            name: body.name,
            displayName: body.display_name
        );

        if (body.contact is not null)
        {
            await branchService.SetContact(branch, body.contact);
        }

        await branchService.SaveChanges();

        return CreatedAtAction(
            nameof(GetBranch),
            new { restaurant_id, branch_id = branch.Id },
            BranchResponse.FromModel(branch)
        );
    }

    [HttpGet("{branch_id}")]
    public async Task<ActionResult<BranchResponse>> GetBranch(Guid restaurant_id, short branch_id)
    {
        var branch = await branchService.GetBranch(restaurant_id, branch_id);

        if (branch is null)
        {
            return NotFound();
        }

        return BranchResponse.FromModel(branch);
    }

    // [HttpPut("{branch_id}")]
    // public async Task<ActionResult> UpdateBranch(Guid restaurant_id, short branch_id, BranchRequest body)
    // {
    //     var branch = await _branchService.Get(restaurant_id, branch_id);

    //     if (branch is null)
    //     {
    //         return NotFound();
    //     }

    //     branch.Name = body.name;
    //     branch.DisplayName = body.display_name;

    //     await _branchService.Save();

    //     return NoContent();
    // }

    [HttpDelete("{branch_id}")]
    public async Task<ActionResult> DeleteBranch(Guid restaurant_id, short branch_id)
    {
        var branch = await branchService.GetBranch(restaurant_id, branch_id);

        if (branch is null)
        {
            return NotFound();
        }

        await branchService.DeleteBranch(branch);
        await branchService.SaveChanges();

        return NoContent();
    }

    [HttpGet("{branch_id}/stocks")]
    public async Task<ActionResult<List<StockDto>>> ListStocks(Guid restaurant_id, short branch_id)
    {
        var branch = await branchService.GetBranch(restaurant_id, branch_id);

        if (branch is null)
        {
            return NotFound();
        }

        return branch.IngredientStocks
            .Select(StockDto.FromModel)
            .ToList();
    }

    [HttpPost("{branch_id}/stocks")]
    public async Task<ActionResult> SetStock(Guid restaurant_id, short branch_id, StockDto body)
    {
        var branch = await branchService.GetBranch(restaurant_id, branch_id);

        if (branch is null)
        {
            return NotFound();
        }

        await branchService.SetStock(branch, body.ingredient_id, body.amount);
        await branchService.SaveChanges();

        return NoContent();
    }

    [HttpDelete("{branch_id}/stocks")]
    public async Task<ActionResult> DeleteStock(Guid restaurant_id, short branch_id, short ingredientId)
    {
        var branch = await branchService.GetBranch(restaurant_id, branch_id);

        if (branch is null)
        {
            return NotFound();
        }

        var stock = await branchService.GetStock(branch, ingredientId);

        if (stock is null)
        {
            return NotFound();
        }

        await branchService.DeleteStock(stock);
        await branchService.SaveChanges();

        return NoContent();
    }

    [HttpGet("{branch_id}/staffs")]
    public async Task<ActionResult<List<StaffResponse>>> ListStaffs(Guid restaurant_id, short branch_id)
    {
        var staffs = await staffService.ListStaffs(restaurant_id, branch_id);

        return staffs.Select(StaffResponse.FromModel).ToList();
    }

    [HttpPost("{branch_id}/staffs")]
    public async Task<ActionResult<StaffResponse>> CreateStaff(Guid restaurant_id, short branch_id, StaffRequest body)
    {
        var branch = await branchService.GetBranch(restaurant_id, branch_id);

        if (branch is null)
        {
            return NotFound();
        }

        var staff = await staffService.CreateStaff(
            branch: branch,
            name: body.name,
            phone: body.phone
        );

        await staffService.SetRoles(
            staff: staff,
            roleIds: body.roles
        );

        await staffService.SaveChanges();

        return CreatedAtAction(
            nameof(GetStaff),
            new { restaurant_id, branch_id, staff_id = staff.Id },
            StaffResponse.FromModel(staff)
        );
    }

    [HttpGet("{branch_id}/staffs/{staff_id}")]
    public async Task<ActionResult<StaffResponse>> GetStaff(Guid restaurant_id, short branch_id, short staff_id)
    {
        var staff = await staffService.GetStaff(restaurant_id, branch_id, staff_id);

        if (staff is null)
        {
            return NotFound();
        }

        return StaffResponse.FromModel(staff);
    }

    // [HttpPut("{branch_id}/{staff_id}")]
    // public async Task<ActionResult> UpdateStaff(short branch_id, short staff_id, StaffRequest staff)
    // {
    //     if (staff_id != staff.Id)
    //     {
    //         return BadRequest();
    //     }

    //     try
    //     {
    //         // await _branchService.Update(staff);
    //         await _branchService.Save();
    //     }
    //     catch (DbUpdateConcurrencyException)
    //     {
    //         if (!await _branchService.Exists(id))
    //         {
    //             return NotFound();
    //         }
    //         else
    //         {
    //             throw;
    //         }
    //     }

    //     return NoContent();
    // }

    [HttpDelete("{branch_id}/staffs/{staff_id}")]
    public async Task<ActionResult> DeleteStaff(Guid restaurant_id, short branch_id, short staff_id)
    {
        var staff = await staffService.GetStaff(restaurant_id, branch_id, staff_id);

        if (staff is null)
        {
            return NotFound();
        }

        await staffService.DeleteStaff(staff);
        await staffService.SaveChanges();

        return NoContent();
    }

    [HttpGet("{branch_id}/tables")]
    public async Task<ActionResult<List<TableResponse>>> ListTables(Guid restaurant_id, short branch_id)
    {
        var tables = await branchService.ListTables(restaurant_id, branch_id);

        return tables.Select(TableResponse.FromModel).ToList();
    }

    [HttpPost("{branch_id}/tables")]
    public async Task<ActionResult<TableResponse>> CreateTable(Guid restaurant_id, short branch_id, TableRequest body)
    {
        var branch = await branchService.GetBranch(restaurant_id, branch_id);

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
            new { restaurant_id, branch_id, table_id = table.Id },
            TableResponse.FromModel(table)
        );
    }

    [HttpGet("{branch_id}/tables/{table_id}")]
    public async Task<ActionResult<TableResponse>> GetTable(Guid restaurant_id, short branch_id, short table_id)
    {
        var table = await branchService.GetTable(restaurant_id, branch_id, table_id);

        if (table is null)
        {
            return NotFound();
        }

        return TableResponse.FromModel(table);
    }

    // [HttpPut("{id}")]
    // public async Task<ActionResult> UpdateTable(string id, Table table)
    // {
    //     if (id != table.Id)
    //     {
    //         return BadRequest();
    //     }

    //     try
    //     {
    //         await _branchService.Update(table);
    //         await _branchService.Save();
    //     }
    //     catch (DbUpdateConcurrencyException)
    //     {
    //         if (!await _branchService.Exists(id))
    //         {
    //             return NotFound();
    //         }
    //         else
    //         {
    //             throw;
    //         }
    //     }

    //     return NoContent();
    // }

    [HttpDelete("{branch_id}/tables/{table_id}")]
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