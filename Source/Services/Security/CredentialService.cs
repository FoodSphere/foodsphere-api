using System.Text;
using Microsoft.IdentityModel.Tokens;
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using DotNetEnv;

namespace FoodSphere.Services;

public interface ICredentialService
{
    public string? this[string key] { get; }
    public SecurityKey GetSecurityKey(string keyName);
    public SigningCredentials SigningCredentials(string keyName);
}

public class BaseCredentialService : ICredentialService
{
    protected readonly ConfigurationManager config;

    public string? this[string key]
    {
        get => config[key];
    }

    public BaseCredentialService(ConfigurationManager configurationManager)
    {
        config = configurationManager;
    }

    public SecurityKey GetSecurityKey(string keyName)
    {
        var key = Encoding.UTF8.GetBytes(this[keyName]!);

        return new SymmetricSecurityKey(key);
    }

    public SigningCredentials SigningCredentials(string keyName)
    {
        var securityKey = GetSecurityKey(keyName);

        return new(securityKey, SecurityAlgorithms.HmacSha256);
    }
}

public class LocalCredentialService : BaseCredentialService
{
    public LocalCredentialService(ConfigurationManager configurationManager) : base(configurationManager)
    {
        Env.Load();
    }
}

public class AzureCredentialService : BaseCredentialService
{
    public AzureCredentialService(ConfigurationManager configurationManager) : base(configurationManager)
    {
        config.AddAzureKeyVault(
            new Uri(config["KeyVault:uri"]!), new DefaultAzureCredential(),
            new AzureKeyVaultConfigurationOptions { ReloadInterval = TimeSpan.FromMinutes(30) });
    }
}