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
                services.AddOptions<EnvConnectionStrings>()
                    .BindConfiguration(EnvConnectionStrings.SectionName)
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
            }

            public void AddGoogleOptions()
            {
                services.AddOptions<EnvGoogle>()
                    .BindConfiguration(EnvGoogle.SectionName)
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
            }

            public void AddMagicLinkOptions()
            {
                services.AddOptions<EnvMagicLink>()
                    .BindConfiguration(EnvMagicLink.SectionName)
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
            }

            public void AddKeyVaultOptions()
            {
                services.AddOptions<EnvKeyVault>()
                    .BindConfiguration(EnvKeyVault.SectionName)
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
            }

            public void AddS3Options()
            {
                services.AddOptions<EnvS3>()
                    .BindConfiguration(EnvS3.SectionName)
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
            }

            public void AddDomainApiOptions()
            {
                services.AddOptions<EnvDomainApi>()
                    .BindConfiguration($"{EnvDomainApi.ParentSectionName}:{EnvDomainApi.SectionName}")
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
            }

            public void AddDomainResourceOptions()
            {
                services.AddOptions<EnvDomainResource>()
                    .BindConfiguration($"{EnvDomainResource.ParentSectionName}:{EnvDomainResource.SectionName}")
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
            }

            public void AddDomainPosOptions()
            {
                services.AddOptions<EnvDomainPos>()
                    .BindConfiguration($"{EnvDomainPos.ParentSectionName}:{EnvDomainPos.SectionName}")
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
            }

            public void AddDomainMasterOptions()
            {
                services.AddOptions<EnvDomainMaster>()
                    .BindConfiguration($"{EnvDomainMaster.ParentSectionName}:{EnvDomainMaster.SectionName}")
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

            }

            public void AddDomainConsumerOptions()
            {
                services.AddOptions<EnvDomainConsumer>()
                    .BindConfiguration($"{EnvDomainConsumer.ParentSectionName}:{EnvDomainConsumer.SectionName}")
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
            }

            public void AddDomainOrderingOptions()
            {
                services.AddOptions<EnvDomainOrdering>()
                    .BindConfiguration($"{EnvDomainOrdering.ParentSectionName}:{EnvDomainOrdering.SectionName}")
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
            }

            // public void AddEnvOptions(params Type[] types)
            // {
            //     foreach (var type in types)
            //     {
            //         if (type == typeof(IEnvOptions))
            //         {
            //             services.AddOptions<EnvDomainApi>()
            //                 .BindConfiguration($"{EnvDomainApi.ParentSectionName}:{EnvDomainApi.SectionName}")
            //                 .ValidateDataAnnotations()
            //                 .ValidateOnStart();

            //             continue;
            //         }

            //         services.AddOptions<EnvDomainOrdering>()
            //             .BindConfiguration($"{EnvDomainOrdering.ParentSectionName}:{EnvDomainOrdering.SectionName}")
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