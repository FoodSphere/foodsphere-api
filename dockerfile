# https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/docker

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY application/foodsphere.application.csproj application/
RUN dotnet restore application/foodsphere.application.csproj

COPY . .
RUN dotnet publish application/foodsphere.application.csproj -c release -o /publish/application --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 as runtime
WORKDIR /app

COPY --from=build /publish/application .

EXPOSE 8080
ENTRYPOINT ["dotnet", "foodsphere.application.dll"]