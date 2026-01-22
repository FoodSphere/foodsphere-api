// using System.Net.Http.Json;
// using System.Text.Json.Nodes;

// namespace FoodSphere.Tests.System;

// public class MasterScenarioSystemTests : SystemTestsBase
// {
//     [Fact]
//     public async Task Restaurant_Flow()
//     {
//         var unique = GetUniqueString();
//         var email = $"{unique}@{TestSeedingGenerator.GetDomain()}";
//         var password = "Password123!";

//         var registerRes = await _client.PostAsJsonAsync("/auth/master", new
//         {
//             email = email,
//             password = password
//         }, TestContext.Current.CancellationToken);

//         registerRes.EnsureSuccessStatusCode();

//         var loginRes = await _client.PostAsJsonAsync("/auth/master/token", new
//         {
//             email = email,
//             password = password
//         }, TestContext.Current.CancellationToken);

//         loginRes.EnsureSuccessStatusCode();

//         var tokenObj = await loginRes.Content.ReadFromJsonAsync<JsonObject>(JsonSerializerOptions, TestContext.Current.CancellationToken);
//         var token = tokenObj?["access_token"]?.ToString();
//         token.Should().NotBeNullOrEmpty();

//         SetJwtToken(token!);

//         var claimsRes = await _client.GetAsync("/current/client/claims", TestContext.Current.CancellationToken);
//         claimsRes.EnsureSuccessStatusCode();

//         var createRestRes = await _client.PostAsJsonAsync("/client/restaurants", new
//         {
//             name = "test_restaurant"
//         }, TestContext.Current.CancellationToken);

//         createRestRes.EnsureSuccessStatusCode();

//         var restObj = await createRestRes.Content.ReadFromJsonAsync<JsonObject>(JsonSerializerOptions, TestContext.Current.CancellationToken);
//         var restaurantId = restObj?["restaurant_id"]?.ToString();

//         restaurantId.Should().NotBeNullOrEmpty();

//         var createBranchRes = await _client.PostAsJsonAsync($"/client/restaurants/{restaurantId}/branches", new
//         {
//             name = "branch 1"
//         }, TestContext.Current.CancellationToken);

//         createBranchRes.EnsureSuccessStatusCode();

//         var getBranchesRes = await _client.GetAsync($"/client/restaurants/{restaurantId}/branches", TestContext.Current.CancellationToken);
//         getBranchesRes.EnsureSuccessStatusCode();

//         var branches = await getBranchesRes.Content.ReadFromJsonAsync<JsonArray>(JsonSerializerOptions, TestContext.Current.CancellationToken);
//         branches.Should().NotBeNullOrEmpty();

//         var branchId = branches?[0]?["id"]?.ToString();
//         var ing1Res = await _client.PostAsJsonAsync($"/client/restaurants/{restaurantId}/ingredients", new
//         {
//             name = "carrot",
//             image_url = "https://foodsphere.com/carrot.jpg",
//             unit = "kg"
//         }, TestContext.Current.CancellationToken);

//         ing1Res.EnsureSuccessStatusCode();

//         var ing1Id = (await ing1Res.Content.ReadFromJsonAsync<JsonObject>(JsonSerializerOptions, TestContext.Current.CancellationToken))?["id"]?.ToString();
//         var menuRes = await _client.PostAsJsonAsync($"/client/restaurants/{restaurantId}/menus", new
//         {
//             name = "test_menu_1",
//             image_url = "",
//             price = 30,
//             ingredients = new[]
//             {
//                 new { ingredient_id = long.Parse(ing1Id!), amount = 20 }
//             }
//         }, TestContext.Current.CancellationToken);

//         menuRes.EnsureSuccessStatusCode();

//         var menuId = (await menuRes.Content.ReadFromJsonAsync<JsonObject>(JsonSerializerOptions, TestContext.Current.CancellationToken))?["id"]?.ToString();
//         var stockRes = await _client.PostAsJsonAsync($"/client/restaurants/{restaurantId}/branches/{branchId}/stocks", new
//         {
//             ingredient_id = long.Parse(ing1Id!),
//             amount = 700
//         }, TestContext.Current.CancellationToken);

//         stockRes.EnsureSuccessStatusCode();

//         var tableRes = await _client.PostAsJsonAsync($"/client/restaurants/{restaurantId}/branches/{branchId}/tables", new
//         {
//             name = "A1"
//         }, TestContext.Current.CancellationToken);

//         tableRes.EnsureSuccessStatusCode();

//         var tableId = (await tableRes.Content.ReadFromJsonAsync<JsonObject>(JsonSerializerOptions, TestContext.Current.CancellationToken))?["id"]?.ToString();
//         var billRes = await _client.PostAsJsonAsync("/client/bills", new
//         {
//             restaurant_id = restaurantId,
//             branch_id = long.Parse(branchId!),
//             table_id = long.Parse(tableId!),
//             pax = 3
//         }, TestContext.Current.CancellationToken);

//         billRes.EnsureSuccessStatusCode();
//     }
// }