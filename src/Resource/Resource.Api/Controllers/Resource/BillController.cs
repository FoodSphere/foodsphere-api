// namespace FoodSphere.Resource.Api.Controller;

// [Route("bills")]
// public class BillController(
//     ILogger<BillController> logger,
//     BillService billService,
//     MenuUpdateService menuService,
//     BranchService branchService
// ) : ResourceControllerBase
// {
//     [HttpGet]
//     public async Task<ActionResult<ICollection<BillResponse>>> ListBills()
//     {
//         var query = billService.QueryBills();

//         var responses = await query
//             .Select(BillResponse.Projection)
//             .ToArrayAsync();

//         return responses;
//     }

//     [HttpGet("{bill_id}/orders")]
//     public async Task<ActionResult<ICollection<OrderResponse>>> ListOrders(Guid bill_id)
//     {
//         var query = billService.QueryOrders(bill_id);

//         var responses = await query
//             .Select(OrderResponse.Projection)
//             .ToArrayAsync();

//         return responses;
//     }
// }