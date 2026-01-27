using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;

// IServiceCollection.PostConfigure()

// # which one is better?
// - https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options
// - https://learn.microsoft.com/en-us/dotnet/core/extensions/options-validation-generator
namespace FoodSphere.Core.Configurations
{
    public static class OptionsExtensions
    {
        extension(IServiceCollection services)
        {
            public void AddConnectionStringsOptions()
            {
                using var sp = services.BuildServiceProvider();
                var config = sp.GetRequiredService<IConfiguration>();

                services.AddOptions<EnvConnectionStrings>()
                    .Bind(config.GetSection(EnvConnectionStrings.SectionName))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
            }

            public void AddGoogleOptions()
            {
                using var sp = services.BuildServiceProvider();
                var config = sp.GetRequiredService<IConfiguration>();

                services.AddOptions<EnvGoogle>()
                    .Bind(config.GetSection(EnvGoogle.SectionName))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
            }

            public void AddMagicLinkOptions()
            {
                using var sp = services.BuildServiceProvider();
                var config = sp.GetRequiredService<IConfiguration>();

                services.AddOptions<EnvMagicLink>()
                    .Bind(config.GetSection(EnvMagicLink.SectionName))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
            }

            public void AddKeyVaultOptions()
            {
                using var sp = services.BuildServiceProvider();
                var config = sp.GetRequiredService<IConfiguration>();

                services.AddOptions<EnvKeyVault>()
                    .Bind(config.GetSection(EnvKeyVault.SectionName))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
            }

            public void AddDomainApiOptions()
            {
                using var sp = services.BuildServiceProvider();
                var config = sp.GetRequiredService<IConfiguration>();

                services.AddOptions<EnvDomainApi>()
                    .Bind(config
                        .GetSection(EnvDomainApi.ParentSectionName)
                        .GetSection(EnvDomainApi.SectionName))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
            }

            public void AddDomainResourceOptions()
            {
                using var sp = services.BuildServiceProvider();
                var config = sp.GetRequiredService<IConfiguration>();

                services.AddOptions<EnvDomainResource>()
                    .Bind(config
                        .GetSection(EnvDomainResource.ParentSectionName)
                        .GetSection(EnvDomainResource.SectionName))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
            }

            public void AddDomainPosOptions()
            {
                using var sp = services.BuildServiceProvider();
                var config = sp.GetRequiredService<IConfiguration>();

                services.AddOptions<EnvDomainPos>()
                    .Bind(config
                        .GetSection(EnvDomainPos.ParentSectionName)
                        .GetSection(EnvDomainPos.SectionName))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
            }

            public void AddDomainMasterOptions()
            {
                using var sp = services.BuildServiceProvider();
                var config = sp.GetRequiredService<IConfiguration>();

                services.AddOptions<EnvDomainMaster>()
                    .Bind(config
                        .GetSection(EnvDomainMaster.ParentSectionName)
                        .GetSection(EnvDomainMaster.SectionName))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

            }

            public void AddDomainConsumerOptions()
            {
                using var sp = services.BuildServiceProvider();
                var config = sp.GetRequiredService<IConfiguration>();

                services.AddOptions<EnvDomainConsumer>()
                    .Bind(config
                        .GetSection(EnvDomainConsumer.ParentSectionName)
                        .GetSection(EnvDomainConsumer.SectionName))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
            }

            public void AddDomainOrderingOptions()
            {
                using var sp = services.BuildServiceProvider();
                var config = sp.GetRequiredService<IConfiguration>();

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

namespace FoodSphere.Common.Options
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