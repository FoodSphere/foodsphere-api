namespace FoodSphere.Consumer.Api.Controller;

[Route("restaurants")]
public class RestaurantController(
    ILogger<RestaurantController> logger,
    RestaurantServiceBase restaurantService
) : FoodSphereControllerBase
{

}