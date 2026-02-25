using Amazon.S3;

namespace FoodSphere.Pos.Api.Configuration;

public static class S3Configuration
{
    public static AmazonS3Client Configure(IServiceProvider sp)
    {
        var envS3 = sp.GetRequiredService<IOptions<EnvS3>>().Value;

        var config = new AmazonS3Config
        {
            // RegionEndpoint = RegionEndpoint.
            ServiceURL = envS3.endpoint_url,
            ForcePathStyle = true,
        };

        return new(envS3.access_key, envS3.secret_key, config);
    }
}