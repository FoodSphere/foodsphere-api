using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FoodSphere.Services;
using FoodSphere.Data.Models;
using FoodSphere.Data;
using FoodSphere.Data.DTOs;

namespace FoodSphere.Controllers.Client;

public class StaffRequest
{
    public required string name { get; set; }
    public List<short> roles { get; set; } = [];
    public string? phone { get; set; }
}

public class StaffResponse
{
    public int id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid restaurant_id { get; set; }
    public short branch_id { get; set; }

    public required string Name { get; set; }
    public string? Phone { get; set; }

    // public StaffStatus status { get; set; }

    public static StaffResponse FromModel(StaffUser model)
    {
        return new StaffResponse
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

public class StaffPortalResponse
{
    public Guid id { get; set; }
    public Guid restaurant_id { get; set; }
    public short branch_id { get; set; }
    public short staff_id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public short? max_usage { get; set; }
    public short usage_count { get; set; }
    public TimeSpan? valid_duration { get; set; }

    public static StaffPortalResponse FromModel(StaffPortal model)
    {
        return new StaffPortalResponse
        {
            id = model.Id,
            restaurant_id = model.RestaurantId,
            branch_id = model.BranchId,
            staff_id = model.StaffId,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            max_usage = model.MaxUsage,
            usage_count = model.UsageCount,
            valid_duration = model.ValidDuration
        };
    }
}

[Route("client/restaurants/{restaurant_id}/branches/{branch_id}/staffs")]
public class StaffController(
    ILogger<StaffController> logger,
    BranchService branchService
) : ClientController
{
    readonly ILogger<StaffController> _logger = logger;
    readonly BranchService _branchService = branchService;

    [HttpPost("{staff_id}/portal")]
    public async Task<ActionResult<StaffPortalResponse>> CreatePortal(Guid restaurant_id, short branch_id, short staff_id)
    {
        var staff = await _branchService.GetStaff(restaurant_id, branch_id, staff_id);

        if (staff is null)
        {
            return NotFound();
        }

        var portal = await _branchService.CreateStaffPortal(staff);

        return StaffPortalResponse.FromModel(portal);
    }

    [HttpPost]
    public async Task<ActionResult<StaffResponse>> CreateStaff(Guid restaurant_id, short branch_id, StaffRequest body)
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
            StaffResponse.FromModel(staff)
        );
    }

    [HttpGet]
    public async Task<ActionResult<List<StaffResponse>>> ListStaffs(Guid restaurant_id, short branch_id)
    {
        var staffs = await _branchService.ListStaffs(restaurant_id, branch_id);

        return staffs
            .Select(StaffResponse.FromModel)
            .ToList();
    }

    [HttpGet("{staff_id}")]
    public async Task<ActionResult<StaffResponse>> GetStaff(Guid restaurant_id, short branch_id, short staff_id)
    {
        var staff = await _branchService.GetStaff(restaurant_id, branch_id, staff_id);

        if (staff is null)
        {
            return NotFound();
        }

        return StaffResponse.FromModel(staff);
    }

    [HttpDelete("{staff_id}")]
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
}