using Microsoft.AspNetCore.Mvc;

namespace FoodSphere.Consumer.Api.Controllers;

[Route("restaurants")]
public class RestaurantController(
    ILogger<RestaurantController> logger,
    RestaurantService restaurantService
) : FoodSphereControllerBase
{

}