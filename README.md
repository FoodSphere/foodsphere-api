
# test
working directory: xUnit set to the Output Directory (net10.0).
```bash
dotnet test
```

# run
working directory: Usually the folder where the .csproj resides (if run from there) or the folder where you executed the command.
```bash
dotnet run --project ./application/foodsphere.application.csproj

docker build -t foodsphere-api .
docker run --rm -p 8080:8080/tcp -e ASPNETCORE_ENVIRONMENT=Development foodsphere-api:latest
```