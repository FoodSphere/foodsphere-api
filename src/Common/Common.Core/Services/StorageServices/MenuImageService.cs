namespace FoodSphere.Common.Service;

public class MenuImageService(
    IStorageService storageService,
    PersistenceService persistenceService,
    MenuRepository menuRepository
) : ServiceBase
{
    public async Task<ResultObject<string>> UploadImage(
        MenuKey key, Stream fileStream, string contentType,
        CancellationToken ct = default)
    {
        var menu = await menuRepository.GetMenu(key, ct);

        if (menu is null)
            return ResultObject.NotFound(key);

        var result = await storageService.Upload(
            "public", "menu", fileStream, contentType);

        if (!result.Successed)
            return ResultObject.Fail(ResultError.External,
                "Failed to upload image.");

        menu.ImageUrl = result.ObjectUrl;

        await persistenceService.Commit(ct);

        return result.ObjectUrl;
    }

    public async Task<ResultObject<string>> GetImageUploadUrl(
        MenuKey key, CancellationToken ct = default)
    {
        var menu = await menuRepository.GetMenu(key, ct);

        if (menu is null)
            return ResultObject.NotFound(key);

        var result = await storageService.GetUploadPreSignedUrl(
            "public", "menu");

        if (!result.Successed)
            return ResultObject.Fail(ResultError.External,
                "Failed to get upload URL.");

        menu.ImageUrl = result.ObjectUrl;

        await persistenceService.Commit(ct);

        return result.Url;
    }

    public async Task<ResultObject> DeleteImage(
        MenuKey key, CancellationToken ct = default)
    {
        var menu = await menuRepository.GetMenu(key, ct);

        if (menu is null)
            return ResultObject.NotFound(key);

        if (menu.ImageUrl is null)
            return ResultObject.Success();

        var result = await storageService.Delete(menu.ImageUrl);

        if (result == false)
            return ResultObject.Fail(ResultError.External,
                "Failed to delete image.");

        menu.ImageUrl = null;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }
}