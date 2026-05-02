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


    /*
   ============================
       ClearAllStocksAsync
   ============================
   */

    [Fact]
    public async Task ClearAllStocksAsync_ClearNoEmptyData_Possitive()
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

        db.Stocks.Add(stock1);
        db.Stocks.Add(stock2);
        db.Stocks.Add(stock3);

        await db.SaveChangesAsync();

        var repo = new StockRepository(db);

        var countBefore = await db.Stocks.CountAsync();

        // Act

        await repo.ClearAllStocksAsync();

        // Assert
        var countAfter = await db.Stocks.CountAsync();
        Assert.Equal(0, countAfter);
        Assert.True(countBefore > countAfter);
    }

    [Fact]
    public async Task ClearAllStocksAsync_ClearEmptyData_Possitive()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Stocks.ExecuteDeleteAsync();

        await db.SaveChangesAsync();

        var repo = new StockRepository(db);

        var countBefore = await db.Stocks.CountAsync();

        // Act

        await repo.ClearAllStocksAsync();

        // Assert
        var countAfter = await db.Stocks.CountAsync();
        Assert.Equal(0, countAfter);
        Assert.True(countBefore == countAfter);
    }

    /*
   ============================
       AddNewStocksAsync
   ============================
   */

    [Fact]
    public async Task AddNewStocksAsync_AddNewData_Possitive()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Stocks.ExecuteDeleteAsync();

        var stock1 = new Stock { Name = "Intel", BankQuantity = 50 };
        db.Stocks.Add(stock1);

        await db.SaveChangesAsync();

        var repo = new StockRepository(db);

        // Act

        var added = new List<Stock> { new Stock { Name = "Nvidia", BankQuantity = 0 } };
        await repo.AddNewStocksAsync(added);

        // Assert

        var stocksInDb = await db.Stocks.ToListAsync();

        Assert.Equal(2, stocksInDb.Count);
        Assert.Contains(stocksInDb, s => s.Name == "Nvidia" && s.BankQuantity == 0);
        Assert.Contains(stocksInDb, s => s.Name == "Intel" && s.BankQuantity == 50);

    }

    [Fact]
    public async Task AddNewStocksAsync_AddEmptyData_Possitive()
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

        var added = new List<Stock> { new Stock { Name = "Nvidia", BankQuantity = 0 },
                                        new Stock { Name = "AMD", BankQuantity = 2 },
                                        new Stock { Name = "Oracle", BankQuantity = 3 } };
        await repo.AddNewStocksAsync(added);

        // Assert

        var stocksInDb = await db.Stocks.ToListAsync();

        Assert.Equal(3, stocksInDb.Count);
        Assert.Contains(stocksInDb, s => s.Name == "Nvidia" && s.BankQuantity == 0);
        Assert.Contains(stocksInDb, s => s.Name == "AMD" && s.BankQuantity == 2);
        Assert.Contains(stocksInDb, s => s.Name == "Oracle" && s.BankQuantity == 3);
    }


    /*
    ============================
        TryDecreaseStockInBankAtomicAsync
    ============================
    */
    [Fact]
    public async Task TryDecreaseStockInBankAtomicAsync_EndingValueIsPossitiveValue_ReturnTrue()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Stocks.ExecuteDeleteAsync();
        db.Stocks.Add(new Stock { Name = "Tesla", BankQuantity = 50 });
        await db.SaveChangesAsync();

        var repo = new StockRepository(db);
        var stock = await db.Stocks.FirstOrDefaultAsync(s => s.Name == "Tesla");
        Assert.NotNull(stock);
        Assert.NotEqual(0, stock.Id);

        // Act
        var result = await repo.TryDecreaseStockInBankAtomicAsync(stock);

        // Assert
        Assert.True(result);
        var updatedStock = await db.Stocks.AsNoTracking().FirstOrDefaultAsync(s => s.Name == "Tesla");
        Assert.Equal(49, updatedStock.BankQuantity);
    }

    [Fact]
    public async Task TryDecreaseStockInBankAtomicAsync_EndingValueIs0_ReturnTrue()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Stocks.ExecuteDeleteAsync();
        db.Stocks.Add(new Stock { Name = "Tesla", BankQuantity = 1 });
        await db.SaveChangesAsync();

        var repo = new StockRepository(db);
        var stock = await db.Stocks.FirstOrDefaultAsync(s => s.Name == "Tesla");
        Assert.NotNull(stock);
        Assert.NotEqual(0, stock.Id);

        // Act
        var result = await repo.TryDecreaseStockInBankAtomicAsync(stock);

        // Assert
        Assert.True(result);
        var updatedStock = await db.Stocks.AsNoTracking().FirstOrDefaultAsync(s => s.Name == "Tesla");
        Assert.Equal(0, updatedStock.BankQuantity);
    }

    [Fact]
    public async Task TryDecreaseStockInBankAtomicAsync_EndingValueIsNegativeValue_ReturnFalse()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Stocks.ExecuteDeleteAsync();
        db.Stocks.Add(new Stock { Name = "Tesla", BankQuantity = 0 });
        await db.SaveChangesAsync();

        var repo = new StockRepository(db);
        var stock = await db.Stocks.FirstOrDefaultAsync(s => s.Name == "Tesla");
        Assert.NotNull(stock);
        Assert.NotEqual(0, stock.Id);

        // Act
        var result = await repo.TryDecreaseStockInBankAtomicAsync(stock);

        // Assert
        Assert.False(result);
        var updatedStock = await db.Stocks.AsNoTracking().FirstOrDefaultAsync(s => s.Name == "Tesla");
        Assert.Equal(0, updatedStock.BankQuantity);
    }

    [Fact]
    public async Task TryDecreaseStockInBankAtomicAsync_InputObjectIsNull_ReturnFalse()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Stocks.ExecuteDeleteAsync();
        db.Stocks.Add(new Stock { Name = "Tesla", BankQuantity = 50 });
        await db.SaveChangesAsync();

        var repo = new StockRepository(db);

        // Act
        var result = await repo.TryDecreaseStockInBankAtomicAsync(null!);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task TryDecreaseStockInBankAtomicAsync_InputObjectDontExistInDb_ReturnFalse()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Stocks.ExecuteDeleteAsync();
        db.Stocks.Add(new Stock { Name = "Tesla", BankQuantity = 50 });
        await db.SaveChangesAsync();

        var repo = new StockRepository(db);
        var nonExistentStock = new Stock { Id = 99999, Name = "NonExistent", BankQuantity = 50 };

        // Act
        var result = await repo.TryDecreaseStockInBankAtomicAsync(nonExistentStock);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task TryDecreaseStockInBankAtomicAsync_AtomicOn100Threads_ReturnTrueFor10()
    {
        // 1. Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        Stock stockToUse;
        await using (var setupDb = new MainDbContext(options))
        {
            await setupDb.Stocks.ExecuteDeleteAsync();
            setupDb.Stocks.Add(new Stock { Name = "Tesla", BankQuantity = 10 });
            await setupDb.SaveChangesAsync();
            stockToUse = await setupDb.Stocks.FirstOrDefaultAsync(s => s.Name == "Tesla");
        }

        int numberOfTasks = 100;
        var tasks = new List<Task<bool>>();

        // 2. Act
        for (int i = 0; i < numberOfTasks; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                await using var taskDb = new MainDbContext(options);
                var taskRepo = new StockRepository(taskDb);
                return await taskRepo.TryDecreaseStockInBankAtomicAsync(stockToUse);
            }));
        }

        var results = await Task.WhenAll(tasks);

        // 3. Assert
        int successfulPurchases = results.Count(r => r == true);

        await using var assertDb = new MainDbContext(options);
        var finalStock = await assertDb.Stocks.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Name == "Tesla");

        Assert.Equal(10, successfulPurchases);
        Assert.Equal(0, finalStock.BankQuantity);
    }


    /*
    ============================
        TryIncreaseStockInBankAtomicAsync 
    ============================
    */
    [Fact]
    public async Task TryIncreaseStockInBankAtomicAsync_IncreaseFromPositiveValue_ReturnTrue()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Stocks.ExecuteDeleteAsync();
        db.Stocks.Add(new Stock { Name = "Tesla", BankQuantity = 50 });
        await db.SaveChangesAsync();

        var repo = new StockRepository(db);
        var stock = await db.Stocks.FirstOrDefaultAsync(s => s.Name == "Tesla");
        Assert.NotNull(stock);
        Assert.NotEqual(0, stock.Id);

        // Act
        var result = await repo.TryIncreaseStockInBankAtomicAsync(stock);

        // Assert
        Assert.True(result);
        var updatedStock = await db.Stocks.AsNoTracking().FirstOrDefaultAsync(s => s.Name == "Tesla");
        Assert.Equal(51, updatedStock.BankQuantity);
    }

    [Fact]
    public async Task TryIncreaseStockInBankAtomicAsync_IncreaseFromZero_ReturnTrue()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Stocks.ExecuteDeleteAsync();
        db.Stocks.Add(new Stock { Name = "Tesla", BankQuantity = 0 });
        await db.SaveChangesAsync();

        var repo = new StockRepository(db);
        var stock = await db.Stocks.FirstOrDefaultAsync(s => s.Name == "Tesla");
        Assert.NotNull(stock);
        Assert.NotEqual(0, stock.Id);

        // Act
        var result = await repo.TryIncreaseStockInBankAtomicAsync(stock);

        // Assert
        Assert.True(result);
        var updatedStock = await db.Stocks.AsNoTracking().FirstOrDefaultAsync(s => s.Name == "Tesla");
        Assert.Equal(1, updatedStock.BankQuantity);
    }

    [Fact]
    public async Task TryIncreaseStockInBankAtomicAsync_InputObjectIsNull_ReturnFalse()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Stocks.ExecuteDeleteAsync();
        db.Stocks.Add(new Stock { Name = "Tesla", BankQuantity = 50 });
        await db.SaveChangesAsync();

        var repo = new StockRepository(db);

        // Act
        var result = await repo.TryIncreaseStockInBankAtomicAsync(null!);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task TryIncreaseStockInBankAtomicAsync_InputObjectDontExistInDb_ReturnFalse()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Stocks.ExecuteDeleteAsync();
        db.Stocks.Add(new Stock { Name = "Tesla", BankQuantity = 50 });
        await db.SaveChangesAsync();

        var repo = new StockRepository(db);
        var nonExistentStock = new Stock { Id = 99999, Name = "NonExistent", BankQuantity = 50 };

        // Act
        var result = await repo.TryIncreaseStockInBankAtomicAsync(nonExistentStock);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task TryIncreaseStockInBankAtomicAsync_AtomicOn100Threads_ReturnTrueFor100()
    {
        // 1. Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        Stock stockToUse;
        await using (var setupDb = new MainDbContext(options))
        {
            await setupDb.Stocks.ExecuteDeleteAsync();
            setupDb.Stocks.Add(new Stock { Name = "Tesla", BankQuantity = 0 });
            await setupDb.SaveChangesAsync();
            stockToUse = await setupDb.Stocks.FirstOrDefaultAsync(s => s.Name == "Tesla");
        }

        int numberOfTasks = 100;
        var tasks = new List<Task<bool>>();

        // 2. Act
        for (int i = 0; i < numberOfTasks; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                await using var taskDb = new MainDbContext(options);
                var taskRepo = new StockRepository(taskDb);
                return await taskRepo.TryIncreaseStockInBankAtomicAsync(stockToUse);
            }));
        }

        var results = await Task.WhenAll(tasks);

        // 3. Assert
        int successfulIncreases = results.Count(r => r == true);

        await using var assertDb = new MainDbContext(options);
        var finalStock = await assertDb.Stocks.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Name == "Tesla");

        Assert.Equal(100, successfulIncreases);
        Assert.Equal(100, finalStock.BankQuantity);
    }

}
