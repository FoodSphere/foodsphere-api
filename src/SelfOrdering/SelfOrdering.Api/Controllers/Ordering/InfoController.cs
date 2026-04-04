namespace FoodSphere.SelfOrdering.Api.Controller;

public class InfoController(
    ILogger<InfoController> logger,
    BranchServiceBase branchService,
    MenuServiceBase menuService,
    TagServiceBase tagService
) : SelfOrderingControllerBase
{
    [HttpGet("restaurant")]
    public async Task<ActionResult<RestaurantBranchResponse>> GetRestaurant()
    {
        var response = await branchService.GetBranch(
            RestaurantBranchResponse.Projection, BranchKey);

        if (response is null)
            return NotFound();

        return response;
    }

    /// <summary>
    /// list tags
    /// </summary>
    [HttpGet("tags")]
    public async Task<ActionResult<ICollection<TagDto>>> ListTags(
        [FromQuery] IReadOnlyCollection<string> type)
    {
        Expression<Func<Tag, bool>> predicate = e =>
            e.RestaurantId == RestaurantId;

        if (type.Count > 0)
            predicate = predicate.And(e => type.Contains(e.Type));

        return await tagService.ListTags(
            TagDto.Projection, predicate);
    }

    [HttpGet("menus")]
    public async Task<ActionResult<ICollection<MenuResponse>>> ListMenus(
        [FromQuery] bool? has_components,
        [FromQuery] bool? stock_availability = true,
        [FromQuery] MenuStatus? status = MenuStatus.Active)
    {
        Expression<Func<Menu, bool>> predicate = e =>
            e.RestaurantId == RestaurantId &&
            e.DeleteTime == null;

        if (has_components is not null)
            predicate = predicate.And(e => e.Components.Any() == has_components.Value);

        if (stock_availability is not null)
            predicate = predicate.And(menu =>
                menu.Ingredients.All(e =>
                    e.Ingredient.Status == IngredientStatus.Active) == stock_availability.Value &&
                menu.Components.All(e =>
                    e.ChildMenu.Ingredients.All(e =>
                        e.Ingredient.Status == IngredientStatus.Active) == stock_availability.Value));

        if (status is not null)
            predicate = predicate.And(e => e.Status == status.Value);

        return await menuService.ListMenus(
            MenuResponse.Projection, predicate);;
    }

    [HttpGet("menus/{menu_id}")]
    public async Task<ActionResult<MenuResponse>> GetMenu(short menu_id)
    {
        var response = await menuService.GetMenu(
            MenuResponse.Projection,
            new(RestaurantId, menu_id));

        if (response is null)
            return NotFound();

        return response;
    }
}