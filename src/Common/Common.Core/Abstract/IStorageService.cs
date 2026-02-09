namespace FoodSphere.Common.Service;

public interface IStorageService
{
    // Task<string> Getkey(string path, string? contentType = null);

    Task<UploadResult> Upload(
        string bucket,
        string key,
        Stream fileStream,
        string? contentType = null
    );

    // Task<Stream> Download(string bucket, string key);

    Task<PresignedUrlResult> GetUploadPreSignedUrl(string bucket, string key, TimeSpan? duration = null);

    Task<string> GetDownloadPreSignedUrl(string bucket, string key, TimeSpan? duration = null);

    Task<bool> Delete(string bucket, string key);
}

public record PresignedUrlResult
{
    public required string Url;
    public required string ObjectUrl;
}

public record UploadResult
{
    public required string ObjectUrl;
    public required bool Successed;
}