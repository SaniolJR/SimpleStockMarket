using Xunit;
using Moq;
using Services;
using Entities;
using Database;
using Microsoft.EntityFrameworkCore;

namespace Tests.RepositoriesTests;

[Collection("postgres")]
public class WalletRepositoryIntegrationTests
{

    private readonly PostgresContainerFixture _fx;
    public WalletRepositoryIntegrationTests(PostgresContainerFixture fx) => _fx = fx;


    /*
    ============================
        GetWalletByIdIncludingWalletStocksAsync
    ============================
    */
    [Fact]
    public async Task GetWalletByIdIncludingWalletStocksAsync_WalletExists_ReturnWallet()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Wallets.ExecuteDeleteAsync();

        Wallet wallet = new Wallet { Id = 10 };
        db.Wallets.Add(wallet);
        await db.SaveChangesAsync();

        var repo = new WalletRepository(db);

        // Act

        var result = await repo.GetWalletByIdIncludingWalletStocksAsync(10);

        // Assert

        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
        Assert.Equal(result, wallet);
    }

    [Fact]
    public async Task GetWalletByIdIncludingWalletStocksAsync_WalletDontExists_ReturnNull()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Wallets.ExecuteDeleteAsync();
        db.Wallets.Add(new Wallet { Id = 10 });
        await db.SaveChangesAsync();

        var repo = new WalletRepository(db);

        // Act

        var result = await repo.GetWalletByIdIncludingWalletStocksAsync(11);

        // Assert

        Assert.Null(result);
    }

    [Fact]
    public async Task GetWalletByIdIncludingWalletStocksAsync_InputArgIs0_ReturnNull()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Wallets.ExecuteDeleteAsync();
        await db.SaveChangesAsync();

        var repo = new WalletRepository(db);

        // Act

        var result = await repo.GetWalletByIdIncludingWalletStocksAsync(10);

        // Assert

        Assert.Null(result);
    }

    [Fact]
    public async Task GetWalletByIdIncludingWalletStocksAsync_InputArgIsNonPositiveNumber_ReturnNull()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Wallets.ExecuteDeleteAsync();
        await db.SaveChangesAsync();

        var repo = new WalletRepository(db);

        // Act

        var result = await repo.GetWalletByIdIncludingWalletStocksAsync(10);

        // Assert

        Assert.Null(result);

    }

    /*
    ============================
        GetStockQuantityInWalletByIdAsync
    ============================
    */

    [Fact]
    public async Task GetStockQuantityInWalletByIdAsync_InputStockDontExists_ReturnMinus1()
    {

        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Wallets.ExecuteDeleteAsync();
        await db.Stocks.ExecuteDeleteAsync();

        var wallet = new Wallet();
        db.Wallets.Add(wallet);
        await db.SaveChangesAsync();

        var repo = new WalletRepository(db);

        // Act
        var result = await repo.GetStockQuantityInWalletByIdAsync(wallet, "NonExistentStock");

        // Assert
        Assert.Equal(-1, result);
    }

    [Fact]
    public async Task GetStockQuantityInWalletByIdAsync_InputNameIsNull_ReturnMinus1()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Wallets.ExecuteDeleteAsync();

        var wallet = new Wallet();
        db.Wallets.Add(wallet);
        await db.SaveChangesAsync();

        var repo = new WalletRepository(db);

        // Act
        var result = await repo.GetStockQuantityInWalletByIdAsync(wallet, null!);

        // Assert
        Assert.Equal(-1, result);
    }

    [Fact]
    public async Task GetStockQuantityInWalletByIdAsync_InputNameIsWhiteSpace_ReturnMinus1()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Wallets.ExecuteDeleteAsync();
        db.Wallets.Add(new Wallet());
        await db.SaveChangesAsync();

        var repo = new WalletRepository(db);
        var wallet = await db.Wallets.FirstOrDefaultAsync();

        // Act
        var result = await repo.GetStockQuantityInWalletByIdAsync(wallet!, " ");

        // Assert
        Assert.Equal(-1, result);
    }

    [Fact]
    public async Task GetStockQuantityInWalletByIdAsync_WalletExistsAndHaveStock_ReturnQuantity()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Wallets.ExecuteDeleteAsync();
        await db.Stocks.ExecuteDeleteAsync();

        var stock = new Stock { Name = "Nvidia", BankQuantity = 0 };
        db.Stocks.Add(stock);
        await db.SaveChangesAsync();

        var wallet = new Wallet { Id = 10 };
        wallet.WalletStocks.Add(new WalletStock { WalletId = 10, StockId = stock.Id, Wallet = wallet, Stock = stock, Quantity = 7 });
        db.Wallets.Add(wallet);
        await db.SaveChangesAsync();

        var repo = new WalletRepository(db);
        var walletFromDb = await db.Wallets.FirstOrDefaultAsync(w => w.Id == wallet.Id);

        // Act
        var result = await repo.GetStockQuantityInWalletByIdAsync(walletFromDb!, "Nvidia");

        // Assert
        Assert.Equal(7, result);
    }

    [Fact]
    public async Task GetStockQuantityInWalletByIdAsync_WalletDontExists_ReturnMinus1()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Wallets.ExecuteDeleteAsync();
        await db.Stocks.ExecuteDeleteAsync();
        await db.SaveChangesAsync();

        var repo = new WalletRepository(db);
        var nonExistentWallet = new Wallet { Id = 999999 };

        // Act
        var result = await repo.GetStockQuantityInWalletByIdAsync(nonExistentWallet, "AnyStock");

        // Assert
        Assert.Equal(-1, result);
    }

    /*
    ============================
        TryIncreaseStockInWalletAtomicAsync
    ============================
    */

    [Fact]
    public async Task TryIncreaseStockInWalletAtomicAsync_WalletIdIsNotGreaterThanZero_ThrowException()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);
        var repo = new WalletRepository(db);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await repo.TryDecreaseStockInWalletAtomicAsync(0, 1));
    }
    [Fact]
    public async Task TryIncreaseStockInWalletAtomicAsync_StockIdIsNotGreaterThanZero_ThrowException()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);
        var repo = new WalletRepository(db);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await repo.TryIncreaseStockInWalletAtomicAsync(1, 0));
    }

    [Fact]
    public async Task TryIncreaseStockInWalletAtomicAsync_IdsAreValidButWalletDontExists_ReturnNull()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);
        await db.Wallets.ExecuteDeleteAsync();
        await db.WalletStocks.ExecuteDeleteAsync();
        await db.Stocks.ExecuteDeleteAsync();
        await db.SaveChangesAsync();

        var stock = new Stock { Name = "X", BankQuantity = 0 };
        db.Stocks.Add(stock);
        await db.SaveChangesAsync();

        var repo = new WalletRepository(db);

        // Act
        var result = await repo.TryIncreaseStockInWalletAtomicAsync(999999, stock.Id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task TryIncreaseStockInWalletAtomicAsync_IdsAreValidButStockDontExists_ReturnNull()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);
        await db.Wallets.ExecuteDeleteAsync();
        await db.WalletStocks.ExecuteDeleteAsync();
        await db.Stocks.ExecuteDeleteAsync();
        await db.SaveChangesAsync();

        var wallet = new Wallet();
        db.Wallets.Add(wallet);
        await db.SaveChangesAsync();

        var repo = new WalletRepository(db);

        // Act
        var result = await repo.TryIncreaseStockInWalletAtomicAsync(wallet.Id, 999999);

        // Assert
        Assert.False(result);
    }
    [Fact]
    public async Task TryIncreaseStockInWalletAtomicAsync_IdsAreValidButStockWalletDontExists_ReturnNull()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);
        await db.Wallets.ExecuteDeleteAsync();
        await db.WalletStocks.ExecuteDeleteAsync();
        await db.Stocks.ExecuteDeleteAsync();
        await db.SaveChangesAsync();

        var wallet = new Wallet();
        db.Wallets.Add(wallet);
        var stock = new Stock { Name = "Y", BankQuantity = 0 };
        db.Stocks.Add(stock);
        await db.SaveChangesAsync();

        var repo = new WalletRepository(db);

        // Act
        var result = await repo.TryIncreaseStockInWalletAtomicAsync(wallet.Id, stock.Id);

        // Assert
        Assert.False(result);
    }
    [Fact]
    public async Task TryIncreaseStockInWalletAtomicAsync_IncreaseProperly_ReturnTrue()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);
        await db.Wallets.ExecuteDeleteAsync();
        await db.WalletStocks.ExecuteDeleteAsync();
        await db.Stocks.ExecuteDeleteAsync();

        var wallet = new Wallet();
        db.Wallets.Add(wallet);
        var stock = new Stock { Name = "Z", BankQuantity = 0 };
        db.Stocks.Add(stock);
        await db.SaveChangesAsync();

        var ws = new WalletStock { WalletId = wallet.Id, StockId = stock.Id, Quantity = 5, Wallet = wallet, Stock = stock };
        db.WalletStocks.Add(ws);
        await db.SaveChangesAsync();

        var repo = new WalletRepository(db);

        // Act
        var result = await repo.TryIncreaseStockInWalletAtomicAsync(wallet.Id, stock.Id);

        // Assert
        Assert.True(result);
        var updated = await db.WalletStocks.AsNoTracking().FirstOrDefaultAsync(w => w.WalletId == wallet.Id && w.StockId == stock.Id);
        Assert.Equal(6, updated.Quantity);
    }

    [Fact]
    public async Task TryIncreaseStockInWalletAtomicAsync_IncreaseProperlyOnMultiThreads_ReturnTrue()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using (var setupDb = new MainDbContext(options))
        {
            await setupDb.Wallets.ExecuteDeleteAsync();
            await setupDb.WalletStocks.ExecuteDeleteAsync();
            await setupDb.Stocks.ExecuteDeleteAsync();
            var wallet = new Wallet();
            setupDb.Wallets.Add(wallet);
            var stock = new Stock { Name = "MT", BankQuantity = 0 };
            setupDb.Stocks.Add(stock);
            await setupDb.SaveChangesAsync();
            setupDb.WalletStocks.Add(new WalletStock { WalletId = wallet.Id, StockId = stock.Id, Quantity = 0, Wallet = wallet, Stock = stock });
            await setupDb.SaveChangesAsync();
        }

        // fetch ids
        int walletId; int stockId;
        await using (var idDb = new MainDbContext(options))
        {
            var wallet = await idDb.Wallets.FirstOrDefaultAsync();
            var stock = await idDb.Stocks.FirstOrDefaultAsync(s => s.Name == "MT");
            walletId = wallet.Id;
            stockId = stock.Id;
        }

        int numberOfTasks = 100;
        var tasks = new List<Task<bool>>();

        // Act
        for (int i = 0; i < numberOfTasks; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                await using var taskDb = new MainDbContext(options);
                var taskRepo = new WalletRepository(taskDb);
                return await taskRepo.TryIncreaseStockInWalletAtomicAsync(walletId, stockId);
            }));
        }

        var results = await Task.WhenAll(tasks);

        // Assert
        int successes = results.Count(r => r);
        await using var assertDb = new MainDbContext(options);
        var final = await assertDb.WalletStocks.AsNoTracking().FirstOrDefaultAsync();
        Assert.Equal(numberOfTasks, successes);
        Assert.Equal(numberOfTasks, final.Quantity);
    }


    /*
    ============================
        TryDecreaseStockInWalletAtomicAsync
    ============================
    */

    [Fact]
    public async Task TryDecreaseStockInWalletAtomicAsync_WalletIdIsNotGreaterThanZero_ThrowException()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);
        var repo = new WalletRepository(db);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await repo.TryDecreaseStockInWalletAtomicAsync(0, 1));
    }
    [Fact]
    public async Task TryDecreaseStockInWalletAtomicAsync_StockIdIsNotGreaterThanZero_ThrowException()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);
        var repo = new WalletRepository(db);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await repo.TryDecreaseStockInWalletAtomicAsync(1, 0));
    }

    [Fact]
    public async Task TryDecreaseStockInWalletAtomicAsync_IdsAreValidButWalletDontExists_ReturnNull()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);
        await db.Wallets.ExecuteDeleteAsync();
        await db.WalletStocks.ExecuteDeleteAsync();
        await db.Stocks.ExecuteDeleteAsync();
        await db.SaveChangesAsync();

        var stock = new Stock { Name = "D_X", BankQuantity = 0 };
        db.Stocks.Add(stock);
        await db.SaveChangesAsync();

        var repo = new WalletRepository(db);

        // Act
        var result = await repo.TryDecreaseStockInWalletAtomicAsync(999999, stock.Id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task TryDecreaseStockInWalletAtomicAsync_IdsAreValidButStockDontExists_ReturnNull()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);
        await db.Wallets.ExecuteDeleteAsync();
        await db.WalletStocks.ExecuteDeleteAsync();
        await db.Stocks.ExecuteDeleteAsync();
        await db.SaveChangesAsync();

        var wallet = new Wallet();
        db.Wallets.Add(wallet);
        await db.SaveChangesAsync();

        var repo = new WalletRepository(db);

        // Act
        var result = await repo.TryDecreaseStockInWalletAtomicAsync(wallet.Id, 999999);

        // Assert
        Assert.False(result);
    }
    [Fact]
    public async Task TryDecreaseStockInWalletAtomicAsync_IdsAreValidButStockWalletDontExists_ReturnNull()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);
        await db.Wallets.ExecuteDeleteAsync();
        await db.WalletStocks.ExecuteDeleteAsync();
        await db.Stocks.ExecuteDeleteAsync();
        await db.SaveChangesAsync();

        var wallet = new Wallet(); db.Wallets.Add(wallet);
        var stock = new Stock { Name = "D_Y", BankQuantity = 0 }; db.Stocks.Add(stock);
        await db.SaveChangesAsync();

        var repo = new WalletRepository(db);

        // Act
        var result = await repo.TryDecreaseStockInWalletAtomicAsync(wallet.Id, stock.Id);

        // Assert
        Assert.False(result);
    }
    [Fact]
    public async Task TryDecreaseStockInWalletAtomicAsync_DecreaseProperlyOn1Threasd_ReturnTrue()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);
        await db.Wallets.ExecuteDeleteAsync();
        await db.WalletStocks.ExecuteDeleteAsync();
        await db.Stocks.ExecuteDeleteAsync();

        var wallet = new Wallet(); db.Wallets.Add(wallet);
        var stock = new Stock { Name = "D_3", BankQuantity = 0 }; db.Stocks.Add(stock);
        await db.SaveChangesAsync();

        var ws = new WalletStock { WalletId = wallet.Id, StockId = stock.Id, Quantity = 2, Wallet = wallet, Stock = stock };
        db.WalletStocks.Add(ws);
        await db.SaveChangesAsync();

        var repo = new WalletRepository(db);

        // Act
        var result = await repo.TryDecreaseStockInWalletAtomicAsync(wallet.Id, stock.Id);

        // Assert
        Assert.True(result);
        var updated = await db.WalletStocks.AsNoTracking().FirstOrDefaultAsync(w => w.WalletId == wallet.Id && w.StockId == stock.Id);
        Assert.Equal(1, updated.Quantity);
    }
    [Fact]
    public async Task TryDecreaseStockInWalletAtomicAsync_ExecuteOn100ThreastAndDecreaseOn10_ReturnTrue()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        int walletId; int stockId;
        await using (var setupDb = new MainDbContext(options))
        {
            await setupDb.Wallets.ExecuteDeleteAsync();
            await setupDb.WalletStocks.ExecuteDeleteAsync();
            await setupDb.Stocks.ExecuteDeleteAsync();
            var wallet = new Wallet(); setupDb.Wallets.Add(wallet);
            var stock = new Stock { Name = "DT_DEC", BankQuantity = 0 }; setupDb.Stocks.Add(stock);
            await setupDb.SaveChangesAsync();
            setupDb.WalletStocks.Add(new WalletStock { WalletId = wallet.Id, StockId = stock.Id, Quantity = 10, Wallet = wallet, Stock = stock });
            await setupDb.SaveChangesAsync();
            walletId = wallet.Id; stockId = stock.Id;
        }

        int numberOfTasks = 100;
        var tasks = new List<Task<bool>>();

        // Act
        for (int i = 0; i < numberOfTasks; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                await using var taskDb = new MainDbContext(options);
                var taskRepo = new WalletRepository(taskDb);
                return await taskRepo.TryDecreaseStockInWalletAtomicAsync(walletId, stockId);
            }));
        }

        var results = await Task.WhenAll(tasks);

        // Assert
        int successes = results.Count(r => r);
        await using var assertDb = new MainDbContext(options);
        var final = await assertDb.WalletStocks.AsNoTracking().FirstOrDefaultAsync();
        Assert.Equal(10, successes);
        Assert.Equal(0, final.Quantity);
    }
    [Fact]
    public async Task TryDecreaseStockInWalletAtomicAsync_TryDecreaseOnStockMarketWithQuantity0_ReturnFalse()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);
        await db.Wallets.ExecuteDeleteAsync();
        await db.WalletStocks.ExecuteDeleteAsync();
        await db.Stocks.ExecuteDeleteAsync();

        var wallet = new Wallet(); db.Wallets.Add(wallet);
        var stock = new Stock { Name = "D0_DEC", BankQuantity = 0 }; db.Stocks.Add(stock);
        await db.SaveChangesAsync();

        db.WalletStocks.Add(new WalletStock { WalletId = wallet.Id, StockId = stock.Id, Quantity = 0, Wallet = wallet, Stock = stock });
        await db.SaveChangesAsync();

        var repo = new WalletRepository(db);

        // Act
        var result = await repo.TryDecreaseStockInWalletAtomicAsync(wallet.Id, stock.Id);

        // Assert
        Assert.False(result);
    }

    /*
    ============================
        CreateNewWalletBasedOnIdAsync
    ============================
    */

    [Fact]
    public async Task CreateNewWalletBasedOnIdAsync_WalletIdIsNotGreaterThan0_ThrowException()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Wallets.ExecuteDeleteAsync();

        var repo = new WalletRepository(db);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await repo.CreateNewWalletBasedOnIdAsync(0));
        await Assert.ThrowsAsync<ArgumentException>(async () => await repo.CreateNewWalletBasedOnIdAsync(-1));
    }
    [Fact]
    public async Task CreateNewWalletBasedOnIdAsync_AddWalletProperly_Possitive()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;

        await using var db = new MainDbContext(options);

        await db.Wallets.ExecuteDeleteAsync();

        var repo = new WalletRepository(db);

        // Act

        await repo.CreateNewWalletBasedOnIdAsync(10);

        // Assert
        var walletInDb = await db.Wallets
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == 10);

        Assert.NotNull(walletInDb);
    }

}
