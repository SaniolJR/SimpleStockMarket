using Xunit;
using Moq;
using Services;
using Entities;
using Database;
using Microsoft.EntityFrameworkCore;

namespace Tests.RepositoriesTests;

[Collection("postgres")]
public class StockRepositoryIntegrationTests
{

    private readonly PostgresContainerFixture _fx;
    public StockRepositoryIntegrationTests(PostgresContainerFixture fx) => _fx = fx;

    [Fact]
    public async Task GetStockByNameAsync_ArgumentIsNull_ReturnNull()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Stocks.ExecuteDeleteAsync();
        db.Stocks.Add(new Stock { Name = "Nvidia", BankQuantity = 100 });
        await db.SaveChangesAsync();

        var repo = new StockRepository(db);
        
        // Act

        var result = await repo.GetStockByNameAsync(null!);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetStockByNameAsync_ArgumentIsWhiteSpace_ReturnNull()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Stocks.ExecuteDeleteAsync();

        var repo = new StockRepository(db);
        // Act

        var result = await repo.GetStockByNameAsync(" ");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetStockByNameAsync_StockDoesntExists_ReturnNull()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Stocks.ExecuteDeleteAsync();
        db.Stocks.Add(new Stock { Name = "Nvidia", BankQuantity = 100 });
        await db.SaveChangesAsync();

        var repo = new StockRepository(db);
        // Act

        var result = await repo.GetStockByNameAsync("AMD");

        // Assert
        Assert.Null(result);
    }
    [Fact]
    public async Task GetStockByNameAsync_StockExists_ReturnStock()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Stocks.ExecuteDeleteAsync();
        var stockTested = new Stock { Name = "Nvidia", BankQuantity = 100 };
        db.Stocks.Add(stockTested);
        await db.SaveChangesAsync();

        var repo = new StockRepository(db);
        // Act

        var result = await repo.GetStockByNameAsync("Nvidia");

        // Assert
        Assert.Equal(result, stockTested);
    }

    [Fact]
    public async Task GetStockByNameAsync_ArgumentDoesntMatchByCaseSensitive_ReturnNull()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Stocks.ExecuteDeleteAsync();
        db.Stocks.Add(new Stock { Name = "Nvidia", BankQuantity = 100 });
        await db.SaveChangesAsync();

        var repo = new StockRepository(db);
        // Act

        var result = await repo.GetStockByNameAsync("nvidia");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetStockByNameAsync_GetSameStockTwiceWithNoChangesInDb_ReturnStockTwice()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Stocks.ExecuteDeleteAsync();
        var stockTested = new Stock { Name = "Nvidia", BankQuantity = 100 };
        db.Stocks.Add(stockTested);
        await db.SaveChangesAsync();

        var repo = new StockRepository(db);
        // Act

        var result1 = await repo.GetStockByNameAsync("Nvidia");
        var result2 = await repo.GetStockByNameAsync("Nvidia");

        // Assert
        Assert.Equal(result1, stockTested);
        Assert.Equal(result2, stockTested);
    }
}
