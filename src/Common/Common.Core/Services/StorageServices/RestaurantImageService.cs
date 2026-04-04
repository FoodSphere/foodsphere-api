namespace FoodSphere.Common.Service;

public class RestaurantImageService(
    IStorageService storageService,
    PersistenceService persistenceService,
    RestaurantRepository restaurantRepository
) : ServiceBase
{
    public async Task<ResultObject<string>> UploadImage(
        RestaurantKey key, Stream fileStream, string contentType,
        CancellationToken ct = default)
    {
        var restaurant = await restaurantRepository.GetRestaurant(key, ct);

        if (restaurant is null)
            return ResultObject.NotFound(key);

        var result = await storageService.Upload(
            "public", "restaurant", fileStream, contentType);

        if (!result.Successed)
            return ResultObject.Fail(ResultError.External,
                "Failed to upload image.");

        restaurant.ImageUrl = result.ObjectUrl;

        await persistenceService.Commit(ct);

        return result.ObjectUrl;
    }

    public async Task<ResultObject<string>> GetImageUploadUrl(
        RestaurantKey key, CancellationToken ct = default)
    {
        var restaurant = await restaurantRepository.GetRestaurant(key, ct);

        if (restaurant is null)
            return ResultObject.NotFound(key);

        var result = await storageService.GetUploadPreSignedUrl(
            "public", "restaurant");

        if (!result.Successed)
            return ResultObject.Fail(ResultError.External,
                "Failed to get upload URL.");

        restaurant.ImageUrl = result.ObjectUrl;

        await persistenceService.Commit(ct);

        return result.Url;
    }

    public async Task<ResultObject> DeleteImage(
        RestaurantKey key, CancellationToken ct = default)
    {
        var restaurant = await restaurantRepository.GetRestaurant(key, ct);

        if (restaurant is null)
            return ResultObject.NotFound(key);

        if (restaurant.ImageUrl is null)
            return ResultObject.Success();

        var result = await storageService.Delete(restaurant.ImageUrl);

        if (result == false)
            return ResultObject.Fail(ResultError.External,
                "Failed to delete image.");

        restaurant.ImageUrl = null;

        await persistenceService.Commit(ct);

        return ResultObject.Success();
    }
}