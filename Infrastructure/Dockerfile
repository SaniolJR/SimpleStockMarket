# STAGE 1: build .NET sdk
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Optimalization - copy project files first (.csproj)
COPY ["SimpleStockMarket/SimpleStockMarket.csproj", "SimpleStockMarket/"]
COPY ["Tests/SimpleStockMarket.Tests/SimpleStockMarket.Tests.csproj", "Tests/SimpleStockMarket.Tests/"]
RUN dotnet restore "SimpleStockMarket/SimpleStockMarket.csproj"

# Copy the rest of the code
COPY . .

# STAGE 2: run tests
FROM build AS testrunner
WORKDIR "/src/Tests/SimpleStockMarket.Tests"
# Run tests - if they fail, build stops
RUN dotnet test "SimpleStockMarket.Tests.csproj" --no-restore

# STAGE 3: publish app
FROM build AS publish
# Publish app in release mode to /app/publish
RUN dotnet publish "SimpleStockMarket/SimpleStockMarket.csproj" -c Release -o /app/publish /p:UseAppHost=false

# STAGE 4: run runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "SimpleStockMarket.dll"]