using System.Text;
using Microsoft.IdentityModel.Tokens;

// IServiceCollection.PostConfigure()

// # which one is better?
// - https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options
// - https://learn.microsoft.com/en-us/dotnet/core/extensions/options-validation-generator
namespace FoodSphere.Common.Configuration
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

            // public void AddEnvOptions(params Type[] types)
            // {
            //     using var sp = services.BuildServiceProvider();
            //     var config = sp.GetRequiredService<IConfiguration>();

            //     foreach (var type in types)
            //     {
            //         if (type == typeof(IEnvOptions))
            //         {
            //             services.AddOptions<EnvDomainApi>()
            //                 .Bind(config
            //                     .GetSection(EnvDomainApi.ParentSectionName)
            //                     .GetSection(EnvDomainApi.SectionName))
            //                 .ValidateDataAnnotations()
            //                 .ValidateOnStart();

            //             continue;
            //         }
            //         services.AddOptions<EnvDomainOrdering>()
            //             .Bind(config
            //                 .GetSection(EnvDomainOrdering.ParentSectionName)
            //                 .GetSection(EnvDomainOrdering.SectionName))
            //             .ValidateDataAnnotations()
            //             .ValidateOnStart();
            //     }
            // }
        }
    }
}

namespace FoodSphere.Common.Options
{
    public static class OptionsExtensions
    {
        extension(NestedDomain nestedDomain)
        {
            public SymmetricSecurityKey GetSecurityKey()
            {
                var key = Encoding.UTF8.GetBytes(nestedDomain.signing_key);

                return new SymmetricSecurityKey(key);
            }

            public SigningCredentials GetSigningCredentials()
            {
                var securityKey = GetSecurityKey(nestedDomain);

                return new(securityKey, SecurityAlgorithms.HmacSha256);
            }
        }
    }
}