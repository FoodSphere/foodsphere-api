using System.Net;
using Amazon.S3;
using Amazon.S3.Model;

namespace FoodSphere.Common.Service;

public class S3StorageService(
    IAmazonS3 s3Client,
    MimeService mimeService,
    IOptions<EnvS3> envS3
) : IStorageService
{
    readonly EnvS3 envS3 = envS3.Value;

    async Task<string> Getkey(string path, string? contentType = null)
    {
        var filename = Guid.CreateVersion7();
        var fileExtension = contentType is null ? null : mimeService.GetExtensionFromContentType(contentType);

        return $"{path}/{filename}{fileExtension}";
    }

    async Task<string> GetUrl(string bucket, string key)
    {
        return $"{envS3.endpoint_url}/{bucket}/{key}";
    }

    public async Task<UploadResult> Upload(
        string bucket,
        string path,
        Stream fileStream,
        string? contentType = null
    ) {
        var key = await Getkey(path, contentType);

        var request = new PutObjectRequest
        {
            BucketName = bucket,
            Key = key,
            InputStream = fileStream,
            ContentType = contentType,
            CannedACL = S3CannedACL.PublicRead
        };

        var response = await s3Client.PutObjectAsync(request);
        var objectUrl = await GetUrl(bucket, key);

        return new UploadResult
        {
            Successed = response.HttpStatusCode == HttpStatusCode.OK,
            ObjectUrl = objectUrl,
        };
    }

    // public async Task<Stream> Download(string bucket, string key)
    // {
    //     var request = new GetObjectRequest
    //     {
    //         BucketName = bucket,
    //         Key = key
    //     };

    //     var response = await s3Client.GetObjectAsync(request);

    //     return response.ResponseStream;
    // }

    public async Task<bool> Delete(string bucket, string key)
    {
        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = bucket,
            Key = key
        };

        var response = await s3Client.DeleteObjectAsync(deleteRequest);

        return response.HttpStatusCode == HttpStatusCode.NoContent;
    }

    public async Task<string> GetDownloadPreSignedUrl(string bucket, string key, TimeSpan? duration = null)
    {
        // extract bucket and key from object url?

        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucket,
            Key = key,
            Expires = DateTime.UtcNow.Add(duration ?? TimeSpan.FromHours(24)),
            Verb = HttpVerb.GET
        };

        var url = await s3Client.GetPreSignedURLAsync(request);

        return url;
    }

    public async Task<PresignedUrlResult> GetUploadPreSignedUrl(string bucket, string path, TimeSpan? duration = null)
    {
        // how to get file extension?
        var key = await Getkey(path);
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucket,
            Key = key,
            Expires = DateTime.UtcNow.Add(duration ?? TimeSpan.FromHours(1)),
            Verb = HttpVerb.PUT,
        };

        var url = await s3Client.GetPreSignedURLAsync(request);
        var objectUrl = await GetUrl(bucket, key);

        return new PresignedUrlResult
        {
            Successed = true,
            ObjectUrl = objectUrl,
            Url = url,
        };
    }
}