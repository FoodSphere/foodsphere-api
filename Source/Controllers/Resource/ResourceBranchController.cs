using Microsoft.AspNetCore.Mvc;
using FoodSphere.Services;
using FoodSphere.Data.Models;
using FoodSphere.Data;
using FoodSphere.Data.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace FoodSphere.Controllers.Resource;

public class ResourceBranchRequest
{
    public ContactDTO? contact { get; set; }

    public required string name { get; set; }
    public string? display_name { get; set; }
}

public class ResourceBranchResponse //: IDTO<Branch, BranchResponse>
{
    public int id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid restaurant_id { get; set; }

    public ContactDTO? contact { get; set; }

    public string? name { get; set; }
    public string? display_name { get; set; }

    public static ResourceBranchResponse FromModel(Branch model)
    {
        return new ResourceBranchResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            restaurant_id = model.RestaurantId,
            contact = ContactDTO.FromModel(model.Contact),
            name = model.Name,
            display_name = model.DisplayName,
        };
    }
}

public class ResourceTableRequest
{
    public string? name { get; set; }
}

public class ResourceTableResponse
{
    public short id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid restaurant_id { get; set; }
    public short branch_id { get; set; }

    public string? name { get; set; }
    public TableStatus status { get; set; }

    public static ResourceTableResponse FromModel(Table model)
    {
        return new ResourceTableResponse
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

public class ResourceStaffRequest
{
    public required string name { get; set; }
    public List<short> roles { get; set; } = [];
    public string? phone { get; set; }
}

public class ResourceStaffResponse
{
    public short id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid restaurant_id { get; set; }
    public short branch_id { get; set; }

    public required string Name { get; set; }
    public string? Phone { get; set; }

    // public StaffStatus status { get; set; }

    public static ResourceStaffResponse FromModel(StaffUser model)
    {
        return new ResourceStaffResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            restaurant_id = model.RestaurantId,
            branch_id = model.BranchId,
            Name = model.Name,
            Phone = model.Phone,
            // status = model.Status,
        };
    }
}

public class ResourceStockDTO
{
    public short ingredient_id { get; set; }
    public decimal amount { get; set; }

    public static ResourceStockDTO FromModel(Stock model)
    {
        return new ResourceStockDTO
        {
            ingredient_id = model.IngredientId,
            amount = model.Amount,
        };
    }
}

[Route("resource/restaurants/{restaurant_id}/branches")]
public class ResourceBranchController(
    ILogger<ResourceBranchController> logger,
    BranchService branchService
) : AdminController
{
    readonly ILogger<ResourceBranchController> _logger = logger;
    readonly BranchService _branchService = branchService;

    [HttpPost]
    public async Task<ActionResult<ResourceBranchResponse>> CreateBranch(Guid restaurant_id, ResourceBranchRequest body)
    {
        var branch = await _branchService.CreateBranch(
            restaurantId: restaurant_id,
            name: body.name,
            displayName: body.display_name
        );

        if (body.contact is not null)
        {
            await _branchService.SetContact(branch, body.contact);
        }

        await _branchService.Save();

        return CreatedAtAction(
            nameof(GetBranch),
            new { restaurant_id, branch_id = branch.Id },
            ResourceBranchResponse.FromModel(branch)
        );
    }

    [HttpGet]
    public async Task<ActionResult<List<ResourceBranchResponse>>> ListBranches(Guid restaurant_id)
    {
        var branches = await _branchService.ListBranches(restaurant_id);

        return branches
            .Select(ResourceBranchResponse.FromModel)
            .ToList();
    }

    [AllowAnonymous]
    [HttpGet("{branch_id}")]
    public async Task<ActionResult<ResourceBranchResponse>> GetBranch(Guid restaurant_id, short branch_id)
    {
        var branch = await _branchService.GetBranch(restaurant_id, branch_id);

        if (branch is null)
        {
            return NotFound();
        }

        return ResourceBranchResponse.FromModel(branch);
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
        var branch = await _branchService.GetBranch(restaurant_id, branch_id);

        if (branch is null)
        {
            return NotFound();
        }

        await _branchService.DeleteBranch(branch);
        await _branchService.Save();

        return NoContent();
    }

    [HttpGet("{branch_id}/stocks")]
    public async Task<ActionResult<List<ResourceStockDTO>>> GetStocks(Guid restaurant_id, short branch_id)
    {
        var branch = await _branchService.GetBranch(restaurant_id, branch_id);

        if (branch is null)
        {
            return NotFound();
        }

        return branch.IngredientStocks
            .Select(ResourceStockDTO.FromModel)
            .ToList();
    }

    [HttpPost("{branch_id}/stocks")]
    public async Task<ActionResult> SetStock(Guid restaurant_id, short branch_id, ResourceStockDTO body)
    {
        var branch = await _branchService.GetBranch(restaurant_id, branch_id);

        if (branch is null)
        {
            return NotFound();
        }

        await _branchService.SetStock(branch, body.ingredient_id, body.amount);
        await _branchService.Save();

        return NoContent();
    }

    [HttpDelete("{branch_id}/stocks")]
    public async Task<ActionResult> DeleteStock(Guid restaurant_id, short branch_id, short ingredientId)
    {
        var branch = await _branchService.GetBranch(restaurant_id, branch_id);

        if (branch is null)
        {
            return NotFound();
        }

        var stock = await _branchService.GetStock(branch, ingredientId);

        if (stock is null)
        {
            return NotFound();
        }

        await _branchService.DeleteStock(stock);
        await _branchService.Save();

        return NoContent();
    }

    [HttpPost("{branch_id}/staffs")]
    public async Task<ActionResult<ResourceStaffResponse>> CreateStaff(Guid restaurant_id, short branch_id, ResourceStaffRequest body)
    {
        var branch = await _branchService.GetBranch(restaurant_id, branch_id);

        if (branch is null)
        {
            return NotFound();
        }

        var staff = await _branchService.CreateStaff(
            branch: branch,
            name: body.name,
            roles: body.roles,
            phone: body.phone
        );

        return CreatedAtAction(
            nameof(GetStaff),
            new { restaurant_id, branch_id, staff_id = staff.Id },
            ResourceStaffResponse.FromModel(staff)
        );
    }

    [HttpGet("{branch_id}/staffs/{staff_id}")]
    public async Task<ActionResult<ResourceStaffResponse>> GetStaff(Guid restaurant_id, short branch_id, short staff_id)
    {
        var staff = await _branchService.GetStaff(restaurant_id, branch_id, staff_id);

        if (staff is null)
        {
            return NotFound();
        }

        return ResourceStaffResponse.FromModel(staff);
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
        var staff = await _branchService.GetStaff(restaurant_id, branch_id, staff_id);

        if (staff is null)
        {
            return NotFound();
        }

        await _branchService.DeleteStaff(staff);
        await _branchService.Save();

        return NoContent();
    }

    [HttpPost("{branch_id}/tables")]
    public async Task<ActionResult<ResourceTableResponse>> CreateTable(Guid restaurant_id, short branch_id, ResourceTableRequest body)
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
            ResourceTableResponse.FromModel(table)
        );
    }

    [HttpGet("{branch_id}/tables/{table_id}")]
    public async Task<ActionResult<ResourceTableResponse>> GetTable(Guid restaurant_id, short branch_id, short table_id)
    {
        var table = await _branchService.GetTable(restaurant_id, branch_id, table_id);

        if (table is null)
        {
            return NotFound();
        }

        return ResourceTableResponse.FromModel(table);
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