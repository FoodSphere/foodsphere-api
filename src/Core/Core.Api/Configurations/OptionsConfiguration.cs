using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;

// # which one is better?
// - https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options
// - https://learn.microsoft.com/en-us/dotnet/core/extensions/options-validation-generator
//
// IServiceCollection.PostConfigure()
//
namespace FoodSphere.Core.Api.Configurations
{
    public static class OptionsExtensions
    {
        extension(IServiceCollection services)
        {
            public void AddConnectionStringsOptions(ConfigurationManager config)
            {
                services.AddOptions<EnvConnectionStrings>()
                    .Bind(config.GetSection(EnvConnectionStrings.SectionName))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
            }

            public void AddGoogleOptions(ConfigurationManager config)
            {
                services.AddOptions<EnvGoogle>()
                    .Bind(config.GetSection(EnvGoogle.SectionName))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
            }

            public void AddMagicLinkOptions(ConfigurationManager config)
            {
                services.AddOptions<EnvMagicLink>()
                    .Bind(config.GetSection(EnvMagicLink.SectionName))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
            }

            public void AddKeyVaultOptions(ConfigurationManager config)
            {
                services.AddOptions<EnvKeyVault>()
                    .Bind(config.GetSection(EnvKeyVault.SectionName))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
            }

            public void AddDomainApiOptions(ConfigurationManager config)
            {
                services.AddOptions<EnvDomainApi>()
                    .Bind(config
                        .GetSection(EnvDomainApi.ParentSectionName)
                        .GetSection(EnvDomainApi.SectionName))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
            }

            public void AddDomainResourceOptions(ConfigurationManager config)
            {
                services.AddOptions<EnvDomainResource>()
                    .Bind(config
                        .GetSection(EnvDomainResource.ParentSectionName)
                        .GetSection(EnvDomainResource.SectionName))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
            }

            public void AddDomainPosOptions(ConfigurationManager config)
            {
                services.AddOptions<EnvDomainPos>()
                    .Bind(config
                        .GetSection(EnvDomainPos.ParentSectionName)
                        .GetSection(EnvDomainPos.SectionName))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
            }

            public void AddDomainMasterOptions(ConfigurationManager config)
            {
                services.AddOptions<EnvDomainMaster>()
                    .Bind(config
                        .GetSection(EnvDomainMaster.ParentSectionName)
                        .GetSection(EnvDomainMaster.SectionName))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

            }

            public void AddDomainConsumerOptions(ConfigurationManager config)
            {
                services.AddOptions<EnvDomainConsumer>()
                    .Bind(config
                        .GetSection(EnvDomainConsumer.ParentSectionName)
                        .GetSection(EnvDomainConsumer.SectionName))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
            }

            public void AddDomainOrderingOptions(ConfigurationManager config)
            {
                services.AddOptions<EnvDomainOrdering>()
                    .Bind(config
                        .GetSection(EnvDomainOrdering.ParentSectionName)
                        .GetSection(EnvDomainOrdering.SectionName))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
            }
        }
    }
}

namespace FoodSphere.Core.Options
{
    public class EnvConnectionStrings
    {
        public const string SectionName = "ConnectionStrings";

        [Required]
        public required string @default { get; init; }
    }

    public class EnvGoogle
    {
        public const string SectionName = "Google";

        [Required]
        public required string client_id { get; init; }
        [Required]
        public required string client_secret { get; init; }
    }

    public class EnvMagicLink
    {
        public const string SectionName = "MagicLink";

        [Required]
        public required string signing_key { get; init; }
        [Required]
        public required string encryption_key { get; init; }
    }

    public class EnvKeyVault
    {
        public const string SectionName = "KeyVault";

        [Required]
        public required string uri { get; init; }
    }

    public class NestedDomain
    {
        [Required]
        public required string url { get; init; }

        [Required]
        public required string signing_key { get; init; }

        public SymmetricSecurityKey GetSecurityKey()
        {
            var key = Encoding.UTF8.GetBytes(signing_key);

            return new SymmetricSecurityKey(key);
        }

        public SigningCredentials GetSigningCredentials()
        {
            var securityKey = GetSecurityKey();

            return new(securityKey, SecurityAlgorithms.HmacSha256);
        }
    }

    public class EnvDomain
    {
        public const string SectionName = "Domain";

        [ValidateObjectMembers]
        public required NestedDomain api { get; init; }

        [ValidateObjectMembers]
        public required NestedDomain resource { get; init; }

        [ValidateObjectMembers]
        public required NestedDomain pos { get; init; }

        [ValidateObjectMembers]
        public required NestedDomain master { get; init; }

        [ValidateObjectMembers]
        public required NestedDomain consumer { get; init; }

        [ValidateObjectMembers]
        public required NestedDomain ordering { get; init; }
    }

    public class EnvDomainApi : NestedDomain
    {
        public const string ParentSectionName = EnvDomain.SectionName;
        public const string SectionName = "api";
    }

    public class EnvDomainResource : NestedDomain
    {
        public const string ParentSectionName = EnvDomain.SectionName;
        public const string SectionName = "resource";
    }

    public class EnvDomainPos : NestedDomain
    {
        public const string ParentSectionName = EnvDomain.SectionName;
        public const string SectionName = "pos";
    }

    public class EnvDomainMaster : NestedDomain
    {
        public const string ParentSectionName = EnvDomain.SectionName;
        public const string SectionName = "master";
    }

    public class EnvDomainConsumer : NestedDomain
    {
        public const string ParentSectionName = EnvDomain.SectionName;
        public const string SectionName = "consumer";
    }

    public class EnvDomainOrdering : NestedDomain
    {
        public const string ParentSectionName = EnvDomain.SectionName;
        public const string SectionName = "ordering";
    }
}