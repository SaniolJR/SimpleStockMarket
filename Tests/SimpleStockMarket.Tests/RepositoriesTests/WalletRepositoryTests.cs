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
        GetWalletByIdAsync
    ============================
    */
    [Fact]
    public async Task GetWalletByIdAsync_WalletExists_ReturnWallet()
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

        var result = await repo.GetWalletByIdAsync(10);

        // Assert

        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
        Assert.Equal(result, wallet);
    }

    [Fact]
    public async Task GetWalletByIdAsync_WalletDontExists_ReturnNull()
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

        var result = await repo.GetWalletByIdAsync(11);

        // Assert

        Assert.Null(result);
    }

    [Fact]
    public async Task GetWalletByIdAsync_InputArgIs0_ReturnNull()
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

        var result = await repo.GetWalletByIdAsync(10);

        // Assert

        Assert.Null(result);
    }

    [Fact]
    public async Task GetWalletByIdAsync_InputArgIsNonPositiveNumber_ReturnNull()
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

        var result = await repo.GetWalletByIdAsync(10);

        // Assert

        Assert.Null(result);

    }

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
        wallet.Stocks.Add(new WalletStock { WalletId = 10, StockId = stock.Id, Wallet = wallet, Stock = stock, Quantity = 7 });
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




}
