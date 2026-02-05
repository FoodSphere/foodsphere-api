namespace FoodSphere.Pos.Test.Integration;

public class OrderingApiTests(SharedAppFixture fixture) : SharedAppTestsBase(fixture)
{
//     [Fact]
    public async Task Post_Order_Should_Succeed()
    {
        var unique = TestSeedingGenerator.GetUniqueString();
        using var builder = CreateTestSeeder();

        var (masterUser, _) = await builder.SeedMasterUserAsync();
        var restaurant = await builder.SeedRestaurantAsync(masterUser.Id);
        var menuItem = await builder.SeedMenuAsync(restaurant.Id);

//         await builder.CommitAsync(TestContext.Current.CancellationToken);
        var requestBody = new OrderDto
        {
            items =
            [
                new()
                {
                    menu_id = menuItem.Id,
                    quantity = 2,
                },
            ]
        };

        // var response = await _client.PostAsJsonAsync($"restaurants/{restaurant.Id}/branches/{1}/{0}", requestBody, TestContext.Current.CancellationToken);
//         response.StatusCode.Should().Be(HttpStatusCode.Created);

//         var reponseBody = await response.Content.ReadFromJsonAsync<Controllers.POS.OrderResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
//         reponseBody.Should().NotBeNull();

//         reponseBody.id.Should().Be(1);
//         reponseBody.bill_id.Should().Be(1);
    }
}