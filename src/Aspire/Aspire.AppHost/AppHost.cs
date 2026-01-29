using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var resourceApi = builder.AddProject<Projects.FoodSphere_Resource_Api>("resource-api");
var posApi = builder.AddProject<Projects.FoodSphere_Pos_Api>("pos-api");
var selfOrderingApi = builder.AddProject<Projects.FoodSphere_SelfOrdering_Api>("ordering-api");
// var consumerApi = builder.AddProject<Projects.FoodSphere_Consumer_Api>("consumer-api");

if (builder.Environment.IsDevelopment())
{
    // var redis = builder.AddRedis("redis");

    var pgServer = builder.AddPostgres("postgres")
        // .WithPgAdmin() // slower?
        .WithPgWeb();

    // https://aspire.dev/id/integrations/databases/postgres/postgres-client/#properties-of-the-postgresql-resources
    // https://aspire.dev/id/integrations/databases/postgres/postgres-host/#using-with-non-net-applications
    // .WithReference(pgDb) => add environment to service: `ConnectionStrings__default=...`
    //
    var pgDb = pgServer.AddDatabase("default");

    var pgMigrator = builder.AddProject<Projects.FoodSphere_Migrator_Npgsql>("migrator")
        .WithReference(pgDb)
        .WaitFor(pgDb);

    resourceApi
        .WithReference(pgDb)
        .WithEndpoint("http", endpoint => {
            endpoint.Port = 0;
        })
        .WithEndpoint("https", endpoint => {
            endpoint.Port = 0;
        })
        .WaitForCompletion(pgMigrator);

    posApi
        .WithReference(pgDb)
        .WithEndpoint("http", endpoint => {
            endpoint.Port = 0;
        })
        .WithEndpoint("https", endpoint => {
            endpoint.Port = 0;
        })
        .WaitForCompletion(pgMigrator);

    selfOrderingApi
        .WithReference(pgDb)
        .WithEndpoint("http", endpoint => {
            endpoint.Port = 0;
        })
        .WithEndpoint("https", endpoint => {
            endpoint.Port = 0;
        })
        .WaitForCompletion(pgMigrator);

    // consumerApi
    //     .WithReference(pgDb)
    //     .WithEndpoint("http", endpoint => {
    //         endpoint.Port = 0;
    //     })
    //     .WithEndpoint("https", endpoint => {
    //         endpoint.Port = 0;
    //     })
    //     .WaitForCompletion(pgMigrator);
}
else if (builder.Environment.IsProduction())
{
    // https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-container-registry
    var registry = builder.AddContainerRegistry("ghcr", "ghcr.io", "foodsphere/foodsphere-api");
    // var registry = builder.AddAzureContainerRegistry("foodsphere");

    // builder.AddAzureContainerAppEnvironment("aca");
    builder.AddKubernetesEnvironment("k8s")
        .WithProperties(k8s =>
        {
            k8s.HelmChartName = "foodsphere-api";
        });

    resourceApi
        .WithContainerRegistry(registry)
        // .WithRemoteImageName("foodsphere/resource-api")
        // .WithRemoteImageTag("latest")
        // .WithHttpHealthCheck("/health")
        .WithExternalHttpEndpoints();

    posApi
        .WithContainerRegistry(registry)
        .WithExternalHttpEndpoints();

    selfOrderingApi
        .WithContainerRegistry(registry)
        .WithExternalHttpEndpoints();

    // consumerApi
    //     .WithContainerRegistry(registry)
    //     .WithExternalHttpEndpoints();
}

builder.Build().Run();