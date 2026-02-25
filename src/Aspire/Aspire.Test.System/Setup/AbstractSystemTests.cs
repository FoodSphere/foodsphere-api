using System.Text.Json;
using System.Text.Json.Serialization;

namespace FoodSphere.Tests.System;

public abstract class SystemTestsBase
{
    protected readonly HttpClient _client;

    public SystemTestsBase()
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:5001")
        };
    }

    protected static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        PropertyNameCaseInsensitive = true
    };

    public void SetJwtToken(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);
    }

    public static string GetUniqueString()
    {
        return "test_" + Guid.CreateVersion7().ToString();
    }
}