namespace FoodSphere.SelfOrdering.Test.Integration;

public class MemberApiTests(SharedAppFixture fixture) : SharedAppTestsBase(fixture)
{
    [Fact]
    public async Task Access_Portal_Succeed()
    {
        using var builder = CreateTestSeeder();

        var (owner, _) = await builder.SeedMasterUser();
        var restaurant = await builder.SeedRestaurant(owner);
        var branch = await builder.SeedBranch(restaurant);
        var table = await builder.SeedTable(branch);
        var bill = await builder.SeedBill(table);
        var portal = await builder.SeedOrderingPortal(bill);

        await builder.Commit();
        var requestBody = new SelfOrderingTokenRequest
        {
            portal_id = portal.Id,
        };

        var response = await _client.PostAsJsonAsync("auth/token", requestBody, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);

        var responseBody = await response.Content.ReadFromJsonAsync<SelfOrderingTokenResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.access_token.Should().NotBeNullOrEmpty();
        // check token validity?
    }
}