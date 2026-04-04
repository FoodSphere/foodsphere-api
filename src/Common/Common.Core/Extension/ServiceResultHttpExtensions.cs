using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using FoodSphere.Common.Service;

namespace FoodSphere.Common.Extension;

public static class ServiceResultHttpExtensions
{
    extension(ResultError errorStatus)
    {
        public int ToHttpStatusCode() => errorStatus switch
        {
            ResultError.NotFound => StatusCodes.Status404NotFound,
            ResultError.Argument => StatusCodes.Status422UnprocessableEntity,
            ResultError.State => StatusCodes.Status409Conflict,
            ResultError.External => StatusCodes.Status424FailedDependency,
            ResultError.Forbidden => StatusCodes.Status403Forbidden,
            ResultError.Authentication => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError,
        };
    }

    extension(ResultErrorObject[] errors)
    {
        public ObjectResult ToActionResult()
        {
            switch (errors.Length)
            {
                case 0:
                    throw new InvalidOperationException(
                        "Cannot convert a successful result to an error action result.");
                case 1:
                    var error = errors.Single();

                    return new ObjectResult(new
                    {
                        message = error.Message,
                        data = error.Data ,
                    })
                    {
                        StatusCode = error.Error.ToHttpStatusCode(),
                    };
                default:
                    return new BadRequestObjectResult(new
                    {
                        errors = errors.Select(
                            err => new
                            {
                                status_code = err.Error.ToHttpStatusCode(),
                                message = err.Message,
                                data = err.Data,
                            })
                    });
            }
        }
    }
}