namespace FoodSphere.SelfOrdering.Api.Controller;

public class InfoController(
    ILogger<InfoController> logger,
    BillService billService,
    RestaurantService restaurantService,
    BranchService branchService,
    MenuService menuService
) : SelfOrderingControllerBase
{
    [HttpGet("restaurant")]
    public async Task<ActionResult<RestaurantBranchResponse>> GetRestaurant()
    {
        var query = await billService.QuerySingleBill(Member.BillId)
            .Select(e => new { e.RestaurantId, e.BranchId })
            .SingleOrDefaultAsync();

        if (query is null)
        {
            return NotFound();
        }

        var response = await branchService.GetBranch(
            query.RestaurantId, query.BranchId,
            RestaurantBranchResponse.Projection);

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }

    [HttpGet("menus")]
    public async Task<ActionResult<ICollection<MenuResponse>>> ListMenus()
    {
        var restaurantId = await billService.QuerySingleBill(Member.BillId)
            .Select(e => e.RestaurantId)
            .SingleOrDefaultAsync();

        var responses = await menuService.QueryMenus()
            .Where(e => e.RestaurantId == restaurantId)
            .Select(MenuResponse.Projection)
            .ToArrayAsync();

        if (responses is null)
        {
            return NotFound();
        }

        return responses;
    }

    [HttpGet("menus/{menu_id}")]
    public async Task<ActionResult<MenuResponse>> GetMenu(short menu_id)
    {
        var restaurantId = await billService.QuerySingleBill(Member.BillId)
            .Select(e => e.RestaurantId)
            .SingleOrDefaultAsync();

        var response = await menuService.GetMenu(restaurantId, menu_id, MenuResponse.Projection);

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }
}