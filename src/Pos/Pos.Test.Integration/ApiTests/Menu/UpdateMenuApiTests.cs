namespace FoodSphere.Pos.Test.Integration;

public class UpdateMenuApiTests(SharedAppFixture fixture) : SharedAppTestsBase(fixture)
{
    [Fact]
    public async Task Add_Menu_Tag_Succeed()
    {
        using var builder = CreateTestSeeder();

        var (owner, _) = await builder.SeedMasterUser();
        var restaurant = await builder.SeedRestaurant(owner);
        var menu = await builder.SeedMenu(restaurant);
        var tag = await builder.SeedTag(restaurant);

        await builder.Commit();
        var requestBody = new AssignTagRequest
        {
            tag_id = tag.Id,
        };

        await Authenticate(owner);

        var response = await _client.PostAsJsonAsync($"restaurants/{restaurant.Id}/menus/{menu.Id}/tags", requestBody, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent, content);
    }
}