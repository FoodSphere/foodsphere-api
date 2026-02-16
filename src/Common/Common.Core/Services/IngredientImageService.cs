namespace FoodSphere.Common.Service;

public class IngredientImageService(
    FoodSphereDbContext context,
    IStorageService storageService
) : ServiceBase(context)
{
    public async Task<string?> UploadImage(Ingredient ingredient, Stream fileStream, string contentType)
    {
        var result = await storageService.Upload("public", "ingredient", fileStream, contentType);

        if (!result.Successed)
        {
            return null;
        }

        ingredient.ImageUrl = result.ObjectUrl;

        return result.ObjectUrl;
    }

    public async Task<string> GetImageUploadUrl(Ingredient ingredient)
    {
        var result = await storageService.GetUploadPreSignedUrl("public", "ingredient");

        // if (!result.Successed)
        // {
        //     return null;
        // }

        ingredient.ImageUrl = result.ObjectUrl;

        return result.Url;
    }
}