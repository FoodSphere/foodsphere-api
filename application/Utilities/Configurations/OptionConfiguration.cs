using System.ComponentModel.DataAnnotations;
using FoodSphere.Configurations.Options;
using Microsoft.Extensions.Options;

// # which is better?
// - https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options
// - https://learn.microsoft.com/en-us/dotnet/core/extensions/options-validation-generator
public static class ServiceOptionExtensions
{
    public static void AddFoodSphereOptions(this IServiceCollection services, ConfigurationManager config)
    {
        services.AddOptions<ConnectionStringsOption>()
            .Bind(config.GetSection(ConnectionStringsOption.SectionName))
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

        services.AddOptions<DomainOption>()
            .Bind(config.GetSection(DomainOption.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}

namespace FoodSphere.Configurations.Options
{
    public class GoogleOption
    {
        public const string SectionName = "Google";

        [Required]
        public required string client_id { get; init; }
        [Required]
        public required string client_secret { get; init; }
    }

    public class ConnectionStringsOption
    {
        public const string SectionName = "ConnectionStrings";

        [Required]
        public required string @default { get; init; }
    }

    public class MagicLinkOption
    {
        public const string SectionName = "MagicLink";

        [Required]
        public required string signing_key { get; init; }
        [Required]
        public required string encryption_key { get; init; }
    }

    public class KeyVaultOption
    {
        public const string SectionName = "KeyVault";

        [Required]
        public required string uri { get; init; }
    }

    public class DomainOption
    {
        public const string SectionName = "Domain";

        [ValidateObjectMembers]
        public required NestdeDomainOption api { get; init; }

        [ValidateObjectMembers]
        public required NestdeDomainOption admin { get; init; }

        [ValidateObjectMembers]
        public required NestdeDomainOption pos { get; init; }

        [ValidateObjectMembers]
        public required NestdeDomainOption master { get; init; }

        [ValidateObjectMembers]
        public required NestdeDomainOption consumer { get; init; }

        [ValidateObjectMembers]
        public required NestdeDomainOption ordering { get; init; }
    }

    public class NestdeDomainOption
    {
        [Required]
        public required string url { get; init; }

        [Required]
        public required string signing_key { get; init; }
    }
}