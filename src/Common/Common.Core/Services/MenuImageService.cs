namespace FoodSphere.Common.Service;

public class MenuImageService(
    FoodSphereDbContext context,
    IStorageService storageService
) : ServiceBase(context)
{
    public async Task<string?> UploadImage(Menu menu, Stream fileStream, string contentType)
    {
        var result = await storageService.Upload("public", "menu", fileStream, contentType);

        if (!result.Successed)
        {
            return null;
        }

        menu.ImageUrl = result.ObjectUrl;

        return result.ObjectUrl;
    }

    public async Task<string> GetImageUploadUrl(Menu menu)
    {
        var result = await storageService.GetUploadPreSignedUrl("public", "menu");

        // if (!result.Successed)
        // {
        //     return null;
        // }

        menu.ImageUrl = result.ObjectUrl;

        return result.Url;
    }
}