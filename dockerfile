FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# COPY *.slnx .
COPY *.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c release -o /publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "foodsphere-api.dll"]