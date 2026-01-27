var builder = DistributedApplication.CreateBuilder(args);

var resourceApi = builder.AddProject<Projects.FoodSphere_Resource_Api>("resource");
var posApi = builder.AddProject<Projects.FoodSphere_Pos_Api>("pos");
var selfOrderingApi = builder.AddProject<Projects.FoodSphere_SelfOrdering_Api>("self-ordering");
var consumerApi = builder.AddProject<Projects.FoodSphere_Consumer_Api>("consumer");

if (builder.Environment.EnvironmentName == "Development")
{
    // var redis = builder.AddRedis("redis");

    var pgServer = builder.AddPostgres("postgres")
        // .WithPgAdmin() // slower?
        .WithPgWeb();

    // https://aspire.dev/id/integrations/databases/postgres/postgres-client/#properties-of-the-postgresql-resources
    // https://aspire.dev/id/integrations/databases/postgres/postgres-host/#using-with-non-net-applications
    // .WithReference(pgDb) to add environment `ConnectionStrings__default=...`
    var pgDb = pgServer.AddDatabase("default");

    var pgMigrator = builder.AddProject<Projects.FoodSphere_Migrator_Npgsql>("migrations")
        .WithReference(pgDb)
        .WaitFor(pgDb);

    posApi
        .WithReference(pgDb)
        .WaitForCompletion(pgMigrator);

    consumerApi
        .WithReference(pgDb)
        .WaitForCompletion(pgMigrator);

    resourceApi
        .WithReference(pgDb)
        .WaitForCompletion(pgMigrator);

    selfOrderingApi
        .WithReference(pgDb)
        .WaitForCompletion(pgMigrator);
}


if (builder.Environment.EnvironmentName == "Production")
{
    builder.AddDockerComposeEnvironment("compose");
    // builder.AddKubernetesEnvironment("k8s");
    // deploy to azure?
}

builder.Build().Run();