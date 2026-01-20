// using System.Net;
// using System.Net.Http.Json;

// namespace FoodSphere.Tests.Integration;

// public class OrderingApiTests(SharedAppFixture fixture) : SharedAppTestsBase(fixture)
// {
//     [Fact]
//     public async Task Post_Order_Should_Succeed()
//     {
//         var unique = TestSeedingGenerator.GetUniqueString();
//         using var builder = CreateTestDataBuilder();

//         var masterUser = await builder.SeedMasterAsync(TestContext.Current.CancellationToken);
//         var restaurant = await builder.SeedRestaurantAsync(masterUser.Id, TestContext.Current.CancellationToken);
//         var menuItem = await builder.SeedMenuAsync(restaurant.Id, TestContext.Current.CancellationToken);

//         await builder.CommitAsync(TestContext.Current.CancellationToken);
//         var requestBody = new Controllers.POS.OrderDTO
//         {
//             items =
//             [
//                 new()
//                 {
//                     menu_id = menuItem.Id,
//                     quantity = 2,
//                 },
//             ]
//         };

//         var response = await _client.PostAsJsonAsync($"/client/restaurants/{restaurant.Id}/branches/{1}/{0}", requestBody, TestContext.Current.CancellationToken);
//         response.StatusCode.Should().Be(HttpStatusCode.Created);

//         var reponseBody = await response.Content.ReadFromJsonAsync<Controllers.POS.OrderResponse>(JsonSerializerOptions, TestContext.Current.CancellationToken);
//         reponseBody.Should().NotBeNull();

//         reponseBody.id.Should().Be(1);
//         reponseBody.bill_id.Should().Be(1);
//     }
// }