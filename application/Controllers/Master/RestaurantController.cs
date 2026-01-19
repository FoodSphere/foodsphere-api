using Microsoft.AspNetCore.Mvc;
using FoodSphere.Services;
using FoodSphere.Data.Models;
using FoodSphere.Data.DTOs;

namespace FoodSphere.Controllers.Client;

public class QuickRestaurantRequest
{
    public ContactDTO? contact { get; set; }
    public required string name { get; set; }
    public string? display_name { get; set; }
    public string? address { get; set; }
    public TimeOnly? opening_time { get; set; }
    public TimeOnly? closing_time { get; set; }
}

public class RestaurantResponse
{
    public Guid id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public ContactDTO contact { get; set; } = null!;

    public required string name { get; set; }
    public string? display_name { get; set; }

    public static RestaurantResponse FromModel(Restaurant model)
    {
        return new RestaurantResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            contact = ContactDTO.FromModel(model.Contact)!,
            name = model.Name,
            display_name = model.DisplayName,
        };
    }
}

public class QuickRestaurantResponse
{
    public Guid restaurant_id { get; set; }
    public ContactDTO restaurant_contact { get; set; } = null!;
    public required string restaurant_name { get; set; }
    public string? restaurant_display_name { get; set; }

    public short branch_id { get; set; }
    public string? branch_name { get; set; }
    public string? branch_address { get; set; }
    public TimeOnly? branch_opening_time { get; set; }
    public TimeOnly? branch_closing_time { get; set; }

    public static QuickRestaurantResponse FromModel(Restaurant restaurant, Branch branch)
    {
        return new QuickRestaurantResponse
        {
            restaurant_id = restaurant.Id,
            restaurant_contact = ContactDTO.FromModel(restaurant.Contact)!,
            restaurant_name = restaurant.Name,
            restaurant_display_name = restaurant.DisplayName,
            branch_id = branch.Id,
            branch_name = branch.Name,
            branch_address = branch.Address,
            branch_opening_time = branch.OpeningTime,
            branch_closing_time = branch.ClosingTime,
        };
    }
}

[ClientAuthorize(FoodSphere.UserType.Master)]
[Route("client/restaurants")]
public class RestaurantController(
    ILogger<RestaurantController> logger,
    RestaurantService restaurantService,
    BranchService branchService
) : ClientController
{
    readonly ILogger<RestaurantController> _logger = logger;
    readonly RestaurantService _restaurantService = restaurantService;
    readonly BranchService _branchService = branchService;

    [HttpPost]
    public async Task<ActionResult<QuickRestaurantResponse>> CreateRestaurant(QuickRestaurantRequest body)
    {
        var restaurant = await _restaurantService.CreateRestaurant(
            ownerId: MasterUser.Id,
            name: body.name,
            displayName: body.display_name
        );

        if (body.contact is not null)
        {
            await _restaurantService.SetContact(restaurant, body.contact);
        }

        var branch = await _branchService.CreateBranch(
            restaurantId: restaurant.Id,
            name: "main"
        );

        await _branchService.Save();

        return CreatedAtAction(
            nameof(Resource.ResourceRestaurantController.GetRestaurant),
            GetContollerName(nameof(Resource.ResourceRestaurantController)),
            new { restaurant_id = restaurant.Id },
            QuickRestaurantResponse.FromModel(restaurant, branch)
        );
    }

    [HttpGet]
    public async Task<ActionResult<List<RestaurantResponse>>> ListMyRestaurants()
    {
        var restaurant = await _restaurantService.ListRestaurants(MasterUser.Id);

        return restaurant
            .Select(RestaurantResponse.FromModel)
            .ToList();
    }

    [HttpGet("{restaurant_id}")]
    public async Task<ActionResult<RestaurantResponse>> GetRestaurant(Guid restaurant_id)
    {
        var restaurant = await _restaurantService.GetRestaurant(restaurant_id);

        if (restaurant is null)
        {
            return NotFound();
        }

        if (restaurant.OwnerId != MasterUser.Id)
        {
            return Forbid();
        }

        return RestaurantResponse.FromModel(restaurant);
    }

    [HttpPost("{restaurant_id}/contact")]
    public async Task<ActionResult<RestaurantResponse>> SetContact(Guid restaurant_id, ContactDTO body)
    {
        var restaurant = await _restaurantService.GetRestaurant(restaurant_id);

        if (restaurant is null)
        {
            return NotFound();
        }

        if (restaurant.OwnerId != MasterUser.Id)
        {
            return Forbid();
        }

        await _restaurantService.SetContact(restaurant, body);
        await _restaurantService.Save();

        return NoContent();
    }

    [HttpDelete("{restaurant_id}")]
    public async Task<ActionResult> DeleteRestaurant(Guid restaurant_id)
    {
        var restaurant = await _restaurantService.GetRestaurant(restaurant_id);

        if (restaurant is null)
        {
            return NotFound();
        }

        if (restaurant.OwnerId != MasterUser.Id)
        {
            return Forbid();
        }

        await _restaurantService.DeleteRestaurant(restaurant);
        await _restaurantService.Save();

        return NoContent();
    }
}