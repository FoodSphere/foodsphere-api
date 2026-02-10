namespace FoodSphere.Pos.Test.Integration;

public class MasterApiTests(SharedAppFixture fixture) : SharedAppTestsBase(fixture)
{
    [Fact]
    public async Task Post_MasterUser_Succeed()
    {
        var unique = TestSeedingGenerator.GetUniqueString();
        // mock email service?

        var requestBody = new MasterRegisterRequest
        {
            email = $"TEST.{unique}@{TestSeedingGenerator.GetDomain()}",
            password = "Password123!",
        };

        var response = await _client.PostAsJsonAsync("auth/master", requestBody, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created, content);

        // var responseBody = await response.Content.ReadFromJsonAsync<MasterRegisterResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        // responseBody.Should().NotBeNull();
    }

    [Fact]
    public async Task Post_Token_Succeed()
    {
        using var builder = CreateTestSeeder();

        var (masterUser, password) = await builder.SeedMasterUser();

        await builder.Commit();
        var requestBody = new MasterTokenRequest
        {
            email = masterUser.Email!,
            password = password,
        };

        var response = await _client.PostAsJsonAsync("auth/master/token", requestBody, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);

        var responseBody = await response.Content.ReadFromJsonAsync<MasterTokenResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.access_token.Should().NotBeNullOrEmpty();
        // check token validity?
    }
}