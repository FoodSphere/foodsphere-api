namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants")]
public class RestaurantController(
    ILogger<RestaurantController> logger,
    RestaurantServiceBase restaurantService,
    RestaurantImageService imageService
) : MasterControllerBase
{
    /// <summary>
    /// list owned restaurants
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ICollection<RestaurantResponse>>> ListOwnedRestaurants(
        [FromQuery] bool? is_deleted = false)
    {
        Expression<Func<Restaurant, bool>> predicate = e =>
            e.OwnerId == MasterId;

        if (is_deleted is not null)
            predicate = predicate.And(e => e.DeleteTime != null == is_deleted.Value);

        return await restaurantService.ListRestaurants(
            RestaurantResponse.Projection, predicate);
    }

    /// <summary>
    /// create restaurant
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<RestaurantResponse>> CreateRestaurant(
        RestaurantRequest body)
    {
        var result = await restaurantService.CreateRestaurant(
            RestaurantResponse.Projection, new(
                new(MasterId),
                body.name,
                body.display_name,
                body.contact));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        var (key, response) = result.Value;

        return CreatedAtAction(
            nameof(InfoController.GetRestaurant),
            GetControllerName(nameof(InfoController)),
            new { restaurant_id = key.Id },
            response);
    }

    /// <summary>
    /// update restaurant
    /// </summary>
    [HttpPut("{restaurant_id}")]
    public async Task<ActionResult> UpdateRestaurant(
        Guid restaurant_id, RestaurantRequest body)
    {
        var result = await restaurantService.UpdateRestaurant(
            new(restaurant_id), new(
                body.name,
                body.display_name,
                body.contact));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }

    /// <summary>
    /// delete restaurant
    /// </summary>
    [HttpDelete("{restaurant_id}")]
    public async Task<ActionResult> DeleteRestaurant(Guid restaurant_id)
    {
        // if (restaurant.OwnerId != MasterId)
        //     return Forbid();

        var result = await restaurantService.DeleteRestaurant(
            new(restaurant_id));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }

    /// <summary>
    /// upload restautant image
    /// </summary>
    [HttpPost("{restaurant_id}/image")]
    public async Task<ActionResult<UploadImageResponse>> UploadImage(
        Guid restaurant_id, IFormFile file)
    {
        ResultObject<string> uploadResult;

        using (var stream = file.OpenReadStream())
            uploadResult = await imageService.UploadImage(
                new(restaurant_id),
                stream, file.ContentType);

        if (!uploadResult.TryGetValue(out var url))
            return uploadResult.Errors.ToActionResult();

        return new UploadImageResponse
        {
            image_url = url
        };
    }

    /// <summary>
    /// generate restaurant image's upload url
    /// </summary>
    [HttpPost("{restaurant_id}/image/upload-url")]
    public async Task<ActionResult<PresignUrlResponse>> GenerateImageUploadUrl(
        Guid restaurant_id)
    {
        var result = await imageService.GetImageUploadUrl(
            new(restaurant_id));

        if (!result.TryGetValue(out var url))
            return result.Errors.ToActionResult();

        return new PresignUrlResponse
        {
            url = url
        };
    }
}