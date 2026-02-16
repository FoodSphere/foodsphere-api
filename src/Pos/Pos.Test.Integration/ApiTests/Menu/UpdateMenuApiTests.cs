namespace FoodSphere.Pos.Test.Integration;

public class UpdateMenuApiTests(SharedAppFixture fixture) : SharedAppTestsBase(fixture)
{
    [Fact]
    public async Task Add_New_Menu_Tag_Succeed()
    {
        using var builder = CreateTestSeeder();

        var (owner, _) = await builder.SeedMasterUser();
        var restaurant = await builder.SeedRestaurant(owner);
        var menu = await builder.SeedMenu(restaurant);
        var tag = await builder.SeedTag(restaurant);

        await builder.Commit();
        await Authenticate(owner);

        var response = await _client.PutAsync($"restaurants/{restaurant.Id}/menus/{menu.Id}/tags/{tag.Id}", null, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created, content);

        var responseBody = await response.Content.ReadFromJsonAsync<TagDto>(JsonSerializerOptions, TestContext.Current.CancellationToken);
        responseBody.Should().NotBeNull();

        responseBody.tag_id.Should().Be(tag.Id);
        responseBody.name.Should().Be(tag.Name);
    }
}