using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FoodSphere.Services;
using FoodSphere.Data.Models;
using FoodSphere.Data.DTOs;

namespace FoodSphere.Controllers.Client;

public class BranchRequest
{
    public ContactDTO? contact { get; set; }

    public required string name { get; set; }
    public string? display_name { get; set; }
    public string? address { get; set; }
    public DateTime? opening_time { get; set; }
    public DateTime? closing_time { get; set; }
}

public class BranchResponse //: IDTO<Branch, BranchResponse>
{
    public int id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid restaurant_id { get; set; }

    public ContactDTO? contact { get; set; }

    public string? name { get; set; }
    public string? display_name { get; set; }
    public string? address { get; set; }
    public DateTime? opening_time { get; set; }
    public DateTime? closing_time { get; set; }

    public static BranchResponse FromModel(Branch model)
    {
        return new BranchResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            restaurant_id = model.RestaurantId,
            contact = ContactDTO.FromModel(model.Contact),
            name = model.Name,
            display_name = model.DisplayName,
            address = model.Address,
            opening_time = model.OpeningTime,
            closing_time = model.ClosingTime,
        };
    }
}

public class ManagerRequest
{
    public Guid restaurant_id;
    public short branch_id;
    public required string master_id;

}

public class ManagerResponse
{
    public Guid restaurant_id;

    public DateTime create_time;
    public DateTime update_time;

    public short branch_id;
    public required string master_id;

    public static ManagerResponse FromModel(Manager model)
    {
        return new ManagerResponse
        {
            restaurant_id = model.RestaurantId,
            branch_id = model.BranchId,
            master_id = model.MasterId,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
        };
    }
}

// # check access
// public class ManageBranchFilter : IAuthorizationFilter
// {
//     public void OnAuthorization(AuthorizationFilterContext context)
//     {
//         var restaurantService = context.HttpContext.RequestServices.GetService<RestaurantService>();
//         var userId = context.HttpContext.User.FindFirstValue(AppClaimTypes.Identity.UserIdClaimType);

//         if (userId is null)
//         {
//             throw new UnauthorizedAccessException();
//         }

//         var restaurantId = Guid.Parse((string)context.RouteData.Values["restaurant_id"]!);

//         if (restaurantService.UserOwnsRestaurant(userId, restaurantId))
//         {
//             context.Result = new ForbidResult();
//         }
//     }
// }

[Route("client/restaurants/{restaurant_id}/branches")]
public class BranchController(
    ILogger<BranchController> logger,
    BranchService branchService
) : ClientController
{
    readonly ILogger<BranchController> _logger = logger;
    readonly BranchService _branchService = branchService;

    [ClientAuthorize(FoodSphere.UserType.Master)]
    [HttpGet]
    public async Task<ActionResult<List<BranchResponse>>> ListBranches(Guid restaurant_id)
    {
        var branches = await _branchService.ListBranches(restaurant_id);

        return branches
            .Select(BranchResponse.FromModel)
            .ToList();
    }

    [HttpGet("{branch_id}")]
    public async Task<ActionResult<BranchResponse>> GetBranch(Guid restaurant_id, short branch_id)
    {
        var branch = await _branchService.GetBranch(restaurant_id, branch_id);

        if (branch is null)
        {
            return NotFound();
        }

        return BranchResponse.FromModel(branch);
    }

    [ClientAuthorize(FoodSphere.UserType.Master)]
    [HttpPost]
    public async Task<ActionResult<BranchResponse>> CreateBranch(Guid restaurant_id, BranchRequest body)
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
            nameof(Resource.ResourceBranchController.GetBranch),
            GetContollerName(nameof(Resource.ResourceBranchController)),
            new { restaurant_id, branch_id = branch.Id },
            BranchResponse.FromModel(branch)
        );
    }

    [ClientAuthorize(FoodSphere.UserType.Master)]
    [HttpGet("managers")]
    public async Task<ActionResult<List<ManagerResponse>>> ListManagers(Guid restaurant_id, short branch_id)
    {
        var managers = await _branchService.ListManagers(restaurant_id, branch_id);

        return managers
            .Select(ManagerResponse.FromModel)
            .ToList();
    }


    [ClientAuthorize(FoodSphere.UserType.Master)]
    [HttpPost("managers")]
    public async Task<ActionResult<ManagerResponse>> CreateManager(Guid restaurant_id, short branch_id, ManagerRequest body)
    {
        var manager = await _branchService.CreateManager(restaurant_id, branch_id, body.master_id);

        await _branchService.Save();

        return ManagerResponse.FromModel(manager);
    }

    [ClientAuthorize(FoodSphere.UserType.Master)]
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
}