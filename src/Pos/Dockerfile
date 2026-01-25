# https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/docker

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ./Pos.Api/FoodSphere.Pos.Api.csproj Pos.Api/
RUN dotnet restore Pos.Api/FoodSphere.Pos.Api.csproj

COPY ./Pos.Api Pos.Api
RUN dotnet publish Pos.Api/FoodSphere.Pos.Api.csproj -c release -o /publish/Pos.Api --no-restore /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /publish/Pos.Api .

EXPOSE 8080
ENTRYPOINT ["dotnet", "FoodSphere.Pos.Api.dll"]