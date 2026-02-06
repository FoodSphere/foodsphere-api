namespace FoodSphere.Consumer.Api.Controller;

[Route("profile")]
public class ProfileController(
    ILogger<ProfileController> logger,
    ProfileService profileService
) : ConsumerControllerBase
{

}