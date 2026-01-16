using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using FoodSphere.Data.Models;

namespace FoodSphere.Tests.Integration;

public class MasterControllerTests(SharedAppFixture fixture) : SharedAppTestsBase(fixture)
{
    async Task<MasterUser> SeedMasterAsync()
    {
        using var scope = CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<MasterUser>>();

        var unique = GetUniqueName();
        var user = new MasterUser()
        {
            UserName = $"{unique}@foodsphere.com",
            Email = $"{unique}@foodsphere.com",
        };

        var result = await userManager.CreateAsync(user, "Password123!");

        return user;
    }

    [Fact]
    public async Task Register_As_Master_Should_Succeed()
    {
        var unique = GetUniqueName();
        // mock email service?

        var requestBody = new Data.DTOs.RegisterRequest
        {
            email = $"{unique}@foodsphere.com",
            password = "Password123!",
        };

        var response = await _client.PostAsJsonAsync("/auth/master", requestBody, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Login_As_Master_Should_Succeed()
    {
        var user = await SeedMasterAsync();

        var requestBody = new Data.DTOs.RegisterRequest
        {
            email = $"{user.Email}",
            password = "Password123!",
        };

        var response = await _client.PostAsJsonAsync("/auth/master/token", requestBody, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseBody = await response.Content.ReadFromJsonAsync<Data.DTOs.TokenResponse>(TestContext.Current.CancellationToken);

        responseBody.Should().NotBeNull();
        responseBody.access_token.Should().NotBeNullOrEmpty();
    }
}