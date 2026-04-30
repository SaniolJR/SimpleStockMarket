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


    /*
    ============================
        GetStockByNameAsync
    ============================
    */
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

    /*
    ============================
        GetAllStocksAvailableAsync
    ============================
    */
    [Fact]
    public async Task GetAllStocksAvailableAsync_ThereAreNoStocks_ReturnEmptyList()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Stocks.ExecuteDeleteAsync();

        await db.SaveChangesAsync();

        var repo = new StockRepository(db);
        // Act

        var result = await repo.GetAllStocksAvailableAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllStocksAvailableAsync_ThereAreSomeStocks_ReturnNonEmptyList()
    {
         // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Stocks.ExecuteDeleteAsync();

        var stock1 = new Stock { Name = "Nvidia", BankQuantity = 0 };
        var stock2 = new Stock { Name = "Intel", BankQuantity = 50 };
        var stock3 = new Stock { Name = "AMD", BankQuantity = 25 };
        var stock4 = new Stock { Name = "Tesla", BankQuantity = 12 };
        var stock5 = new Stock { Name = "Meta", BankQuantity = 2 };
        var stock6 = new Stock { Name = "Google", BankQuantity = 5 };

        db.Stocks.Add(stock1);
        db.Stocks.Add(stock2);
        db.Stocks.Add(stock3);
        db.Stocks.Add(stock4);
        db.Stocks.Add(stock5);
        db.Stocks.Add(stock6);

        await db.SaveChangesAsync();

        var repo = new StockRepository(db);
        // Act

        var result = await repo.GetAllStocksAvailableAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetAllStocksAvailableAsync_CheckStocksQuantity_Possitive()
    {
          // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Stocks.ExecuteDeleteAsync();

        var stock1 = new Stock { Name = "Nvidia", BankQuantity = 0 };
        var stock2 = new Stock { Name = "Intel", BankQuantity = 50 };
        var stock3 = new Stock { Name = "AMD", BankQuantity = 25 };
        var stock4 = new Stock { Name = "Tesla", BankQuantity = 12 };
        var stock5 = new Stock { Name = "Meta", BankQuantity = 2 };
        var stock6 = new Stock { Name = "Google", BankQuantity = 5 };

        db.Stocks.Add(stock1);
        db.Stocks.Add(stock2);
        db.Stocks.Add(stock3);
        db.Stocks.Add(stock4);
        db.Stocks.Add(stock5);
        db.Stocks.Add(stock6);

        await db.SaveChangesAsync();

        var repo = new StockRepository(db);

        // Act

        var result = await repo.GetAllStocksAvailableAsync();

        // Assert
        
        Assert.All(result, s => Assert.True(s.BankQuantity >= 0));
        Assert.Contains(result, s => s.Name == "Intel" && s.BankQuantity == 50);
        Assert.Contains(result, s => s.Name == "AMD" && s.BankQuantity == 25);
        Assert.Contains(result, s => s.Name == "Tesla" && s.BankQuantity == 12);
        Assert.Contains(result, s => s.Name == "Meta" && s.BankQuantity == 2);
        Assert.Contains(result, s => s.Name == "Google" && s.BankQuantity == 5);
        Assert.Contains(result, s => s.Name == "Nvidia" && s.BankQuantity == 0);

    }

}
