// namespace FoodSphere.Resource.Api.Controller;

// [Route("restaurants/{restaurant_id}/branches")]
// public class BranchController(
//     ILogger<BranchController> logger,
//     BranchService branchService,
//     StaffService staffService
// ) : ResourceControllerBase
// {
//     [HttpGet]
//     public async Task<ActionResult<List<BranchResponse>>> ListBranches(Guid restaurant_id)
//     {
//         var branches = await branchService.ListBranches(restaurant_id);

//         return branches
//             .Select(BranchResponse.FromModel)
//             .ToList();
//     }

//     [HttpGet("{branch_id}/stocks")]
//     public async Task<ActionResult<List<StockDto>>> ListStocks(Guid restaurant_id, short branch_id)
//     {
//         var branch = await branchService.GetBranch(restaurant_id, branch_id);

//         if (branch is null)
//         {
//             return NotFound();
//         }

//         return branch.IngredientStocks
//             .Select(StockDto.FromModel)
//             .ToList();
//     }

//     [HttpGet("{branch_id}/staffs")]
//     public async Task<ActionResult<List<StaffResponse>>> ListStaffs(Guid restaurant_id, short branch_id)
//     {
//         var staffs = await staffService.ListStaffs(restaurant_id, branch_id);

//         return staffs.Select(StaffResponse.FromModel).ToList();
//     }

//     [HttpGet("{branch_id}/tables")]
//     public async Task<ActionResult<List<TableResponse>>> ListTables(Guid restaurant_id, short branch_id)
//     {
//         var tables = await branchService.ListTables(restaurant_id, branch_id);

//         return tables.Select(TableResponse.FromModel).ToList();
//     }
// }