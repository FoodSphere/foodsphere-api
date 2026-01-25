using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FoodSphere.Pos.Api.Controllers;

// check master have access to branch?
public class ManageBranchFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var sp = context.HttpContext.RequestServices;
        var restaurantService = sp.GetService<RestaurantService>();

        var userId = context.HttpContext.User.FindFirstValue(FoodSphereClaimType.Identity.UserIdClaimType);

        if (userId is null)
        {
            throw new UnauthorizedAccessException();
        }

        // var restaurantId = Guid.Parse((string)context.RouteData.Values["restaurant_id"]!);

        // if (restaurantService.UserOwnsRestaurant(userId, restaurantId))
        // {
        //     context.Result = new ForbidResult();
        // }
    }
}

[Route("restaurants/{restaurant_id}/branches")]
public class MasterBranchController(
    ILogger<MasterBranchController> logger,
    BranchService branchService
) : MasterControllerBase
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
            displayName: body.display_name,
            address: body.address,
            openingTime: body.opening_time,
            closingTime: body.closing_time
        );

        if (body.contact is not null)
        {
            await branchService.SetContact(branch, body.contact);
        }

        await branchService.SaveAsync();

        return CreatedAtAction(
            nameof(StaffAccessController.GetBranch),
            GetControllerName(nameof(StaffAccessController)),
            new { restaurant_id, branch_id = branch.Id },
            BranchResponse.FromModel(branch)
        );
    }

    [HttpGet("managers")]
    public async Task<ActionResult<List<ManagerResponse>>> ListManagers(Guid restaurant_id, short branch_id)
    {
        var managers = await branchService.ListManagers(restaurant_id, branch_id);

        return managers
            .Select(ManagerResponse.FromModel)
            .ToList();
    }

    [HttpPost("managers")]
    public async Task<ActionResult<ManagerResponse>> CreateManager(Guid restaurant_id, short branch_id, ManagerRequest body)
    {
        var manager = await branchService.CreateManager(restaurant_id, branch_id, body.master_id);

        await branchService.SaveAsync();

        return ManagerResponse.FromModel(manager);
    }

    [HttpDelete("{branch_id}")]
    public async Task<ActionResult> DeleteBranch(Guid restaurant_id, short branch_id)
    {
        var branch = await branchService.GetBranch(restaurant_id, branch_id);

        if (branch is null)
        {
            return NotFound();
        }

        await branchService.DeleteBranch(branch);
        await branchService.SaveAsync();

        return NoContent();
    }
}