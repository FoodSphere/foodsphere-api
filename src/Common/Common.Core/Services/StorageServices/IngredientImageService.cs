namespace FoodSphere.Common.Service;

public class IngredientImageService(
    IStorageService storageService,
    PersistenceService persistenceService,
    IngredientRepository ingredientRepository
) : ServiceBase
{
    public async Task<ResultObject<string>> UploadImage(
        IngredientKey key, Stream fileStream, string contentType,
        CancellationToken ct = default)
    {
        var ingredient = await ingredientRepository.GetIngredient(key, ct);

        if (ingredient is null)
            return ResultObject.NotFound(key);

        var result = await storageService.Upload(
            "public", "ingredient", fileStream, contentType);

        if (!result.Successed)
            return ResultObject.Fail(ResultError.External,
                "Failed to upload image.");

        ingredient.ImageUrl = result.ObjectUrl;

        await persistenceService.Commit(ct);

        return result.ObjectUrl;
    }

    public async Task<ResultObject<string>> GetImageUploadUrl(
        IngredientKey key, CancellationToken ct = default)
    {
        var ingredient = await ingredientRepository.GetIngredient(key, ct);

        if (ingredient is null)
            return ResultObject.NotFound(key);

        var result = await storageService.GetUploadPreSignedUrl(
            "public", "ingredient");

        if (!result.Successed)
            return ResultObject.Fail(ResultError.External,
                "Failed to get upload URL.");

        ingredient.ImageUrl = result.ObjectUrl;

        await persistenceService.Commit(ct);

        return result.Url;
    }

    public async Task<ResultObject> DeleteImage(
        IngredientKey key, CancellationToken ct = default)
    {
        var ingredient = await ingredientRepository.GetIngredient(key, ct);

        if (ingredient is null)
            return ResultObject.NotFound(key);

        if (ingredient.ImageUrl is null)
            return ResultObject.Success();

        var result = await storageService.Delete(ingredient.ImageUrl);

        if (result == false)
            return ResultObject.Fail(ResultError.External,
                "Failed to delete image.");

        ingredient.ImageUrl = null;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }
}