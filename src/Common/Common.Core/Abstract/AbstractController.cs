using Microsoft.AspNetCore.Mvc;

namespace FoodSphere.Common.Api;

[ApiController]
public abstract class FoodSphereControllerBase : ControllerBase
{
    protected static string GetControllerName(string name)
    {
        // get class dynamically, directly -> use type argument
        return name.EndsWith("Controller") ? name[..^"Controller".Length] : name;
    }
}