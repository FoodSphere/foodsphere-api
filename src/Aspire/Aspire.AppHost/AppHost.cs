var builder = DistributedApplication.CreateBuilder(args);

// var postgres = builder.AddPostgres("postgres")
//     .WithPgAdmin();

// var db = postgres.AddDatabase("default");
// var cache = builder.AddRedis("redis");

builder.AddKubernetesEnvironment("k8s");
// builder.AddDockerComposeEnvironment("compose");

var posApi = builder.AddProject<Projects.FoodSphere_Pos_Api>("pos");

var consumerApi = builder.AddProject<Projects.FoodSphere_Consumer_Api>("consumer");

var resourceApi = builder.AddProject<Projects.FoodSphere_Resource_Api>("resource");

var selfOrderingApi = builder.AddProject<Projects.FoodSphere_SelfOrdering_Api>("self-ordering");

builder.Build().Run();