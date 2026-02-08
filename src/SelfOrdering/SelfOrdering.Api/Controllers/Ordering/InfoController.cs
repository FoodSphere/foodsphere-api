namespace FoodSphere.SelfOrdering.Api.Controller;

public class InfoController(
    ILogger<InfoController> logger,
    RestaurantService restaurantService,
    BranchService branchService,
    MenuService menuService
) : SelfOrderingControllerBase
{
    [HttpGet("restaurant")]
    public async Task<ActionResult<RestaurantBranchResponse>> GetRestaurant()
    {
        var restaurantId = Member.Bill.RestaurantId;
        var branchId = Member.Bill.BranchId;

        var restaurant = await restaurantService.GetRestaurant(restaurantId);
        var branch = await branchService.GetBranch(restaurantId, branchId);

        if (branch is null || restaurant is null)
        {
            return NotFound();
        }

        return RestaurantBranchResponse.FromModel(restaurant, branch);
    }

    [HttpGet("menus")]
    public async Task<ActionResult<List<MenuResponse>>> ListMenus()
    {
        var restaurantId = Member.Bill.RestaurantId;

        var menu = await menuService.ListMenus(restaurantId);

        if (menu is null)
        {
            return NotFound();
        }

        return menu.Select(MenuResponse.FromModel).ToList();
    }

    [HttpGet("menus/{menu_id}")]
    public async Task<ActionResult<MenuResponse>> GetMenu(short menu_id)
    {
        var restaurantId = Member.Bill.RestaurantId;

        var menu = await menuService.FindMenu(restaurantId, menu_id);

        if (menu is null)
        {
            return NotFound();
        }

        return MenuResponse.FromModel(menu);
    }
}