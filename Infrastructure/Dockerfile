# STAGE 1: build .NET sdk
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Optimalization - copy first project file (.csproj)
COPY ["SimpleStockMarket/SimpleStockMarket.csproj", "./SimpleStockMarket/"]
RUN dotnet restore "./SimpleStockMarket/SimpleStockMarket.csproj"

# Copy the rest of the code
COPY SimpleStockMarket/. ./SimpleStockMarket/

# Publish app in release mode to /app/publish
RUN dotnet publish "SimpleStockMarket/SimpleStockMarket.csproj" -c Release -o /app/publish /p:UseAppHost=false

# STAGE 2: run runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "SimpleStockMarket.dll"]