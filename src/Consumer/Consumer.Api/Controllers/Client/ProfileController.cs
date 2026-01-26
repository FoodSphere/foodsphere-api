using Microsoft.AspNetCore.Mvc;

namespace FoodSphere.Consumer.Api.Controllers;

[Route("profile")]
public class ProfileController(
    ILogger<ProfileController> logger,
    ProfileService profileService
) : ConsumerControllerBase
{

}