using FoodSphere.Configurations.Options;

public static class ServiceOptionExtensions
{
    public static void AddFoodSphereOptions(this IServiceCollection services, ConfigurationManager config)
    {
        services.AddOptions<ConnectionStringsOption>()
            .Bind(config.GetSection(ConnectionStringsOption.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<JwtOption>()
            .Bind(config.GetSection(JwtOption.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<MagicLinkOption>()
            .Bind(config.GetSection(MagicLinkOption.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<GoogleOption>()
            .Bind(config.GetSection(GoogleOption.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}

namespace FoodSphere.Configurations.Options
{
    public class GoogleOption
    {
        public const string SectionName = "Google";

        public required string client_id { get; init; }
        public required string client_secret { get; init; }
    }

    public class ConnectionStringsOption
    {
        public const string SectionName = "ConnectionStrings";

        public required string foodsphere { get; init; }
    }

    public class JwtOption
    {
        public const string SectionName = "Jwt";

        public required string signing_key { get; init; }
    }

    public class MagicLinkOption
    {
        public const string SectionName = "MagicLink";

        public required string signing_key { get; init; }
        public required string encryption_key { get; init; }
    }

    public class KeyVaultOption
    {
        public const string SectionName = "KeyVault";

        public required string uri { get; init; }
    }
}