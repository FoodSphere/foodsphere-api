namespace FoodSphere.Pos.Api.Controller;

[Route("s/restaurants")]
public class SingleRestaurantController(
    ILogger<SingleRestaurantController> logger,
    PersistenceService persistenceService,
    RestaurantServiceBase restaurantService,
    BranchServiceBase branchService,
    AccessControlService accessControl
) : MasterControllerBase
{
    /// <summary>
    /// create restaurant
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SingleRestaurantResponse>> CreateRestaurant(
        SingleRestaurantRequest body)
    {
        await using var transaction = await persistenceService.BeginTransaction();

        var retaurantResult = await restaurantService.CreateRestaurant(
            e => true, new(
                new(MasterId),
                body.name,
                body.display_name,
                body.contact));

        if (retaurantResult.IsFailed)
            return retaurantResult.Errors.ToActionResult();

        var (restaurantKey, _) = retaurantResult.Value;

        var branchResult = await branchService.CreateBranch(
            SingleRestaurantResponse.Projection, new(
                restaurantKey,
                "default",
                "",
                body.address,
                body.opening_time,
                body.closing_time,
                body.contact));

        if (branchResult.IsFailed)
            return branchResult.Errors.ToActionResult();

        var (_, response) = branchResult.Value;

        // SeedSimpleRole

        await transaction.CommitAsync();

        return CreatedAtAction(
            nameof(SingleInfoController.GetRestaurant),
            GetControllerName(nameof(SingleInfoController)),
            new { restaurant_id = restaurantKey.Id },
            response);
    }

    /// <summary>
    /// list owned restaurants
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ICollection<SingleRestaurantResponse>>> ListOwnedRestaurants(
        [FromQuery] bool? is_deleted = false)
    {
        Expression<Func<Branch, bool>> predicate = e =>
            e.Restaurant.OwnerId == MasterId &&
            e.Id == 1;

        if (is_deleted is not null)
            predicate = predicate.And(e =>
                e.Restaurant.DeleteTime != null == is_deleted.Value ||
                e.DeleteTime != null == is_deleted.Value);

        return await branchService.ListBranches(
            SingleRestaurantResponse.Projection, predicate);
    }

    /// <summary>
    /// update restaurant
    /// </summary>
    [HttpPut("{restaurant_id}")]
    public async Task<ActionResult<SingleRestaurantResponse>> UpdateRestaurant(
        Guid restaurant_id, SingleRestaurantRequest body)
    {
        var authorizeResult = await accessControl.Authorize(HttpContext,
            PERMISSION.Restaurant.Settings.UPDATE);

        if (authorizeResult.IsFailed)
            return authorizeResult.Errors.ToActionResult();

        await using var transaction = await persistenceService.BeginTransaction();

        var restaurantResult = await restaurantService.UpdateRestaurant(
            new(restaurant_id), new(
                body.name,
                body.display_name,
                body.contact));

        if (restaurantResult.IsFailed)
            return restaurantResult.Errors.ToActionResult();

        var branchResult = await branchService.UpdateBranch(
             new(restaurant_id, 1), new(
                 body.name,
                 body.display_name,
                 body.address,
                 body.opening_time,
                 body.closing_time,
                 body.contact));

        if (branchResult.IsFailed)
            return branchResult.Errors.ToActionResult();

        await transaction.CommitAsync();

        return NoContent();
    }
}