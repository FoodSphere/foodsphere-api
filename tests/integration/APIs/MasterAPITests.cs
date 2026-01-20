using System.Net;
using System.Net.Http.Json;

namespace FoodSphere.Tests.Integration;

public class MasterApiTests(SharedAppFixture fixture) : SharedAppTestsBase(fixture)
{
    [Fact]
    public async Task Post_User_Should_Succeed()
    {
        var unique = TestSeedingGenerator.GetUniqueString();
        // mock email service?

        var requestBody = new Data.DTOs.RegisterRequest
        {
            email = $"TEST.{unique}@{TestSeedingGenerator.GetDomain()}",
            password = "Password123!",
        };

        var response = await _client.PostAsJsonAsync("/auth/master", requestBody, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created, content);

        // var responseBody = await response.Content.ReadFromJsonAsync<>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        // responseBody.Should().NotBeNull();
    }

    [Fact]
    public async Task Post_Token_Should_Succeed()
    {
        using var builder = CreateSeedingBuilder();

        var (masterUser, password) = await builder.SeedMasterUserAsync();

        await builder.CommitAsync();
        var requestBody = new Data.DTOs.TokenRequest
        {
            email = masterUser.Email!,
            password = password,
        };

        var response = await _client.PostAsJsonAsync("/auth/master/token", requestBody, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);

        var responseBody = await response.Content.ReadFromJsonAsync<Data.DTOs.TokenResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.access_token.Should().NotBeNullOrEmpty();
        // check token validity?
    }
}