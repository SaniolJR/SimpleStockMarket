using Moq;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using Services;
using Entities;

public class BankServiceTests
{
    private readonly Mock<IStockRepository> _stockRepositoryMock;
    private readonly Mock<IWalletRepository> _walletRepositoryMock;
    private readonly BankService _bankService;

    public BankServiceTests()
    {
        _stockRepositoryMock = new Mock<IStockRepository>();
        _walletRepositoryMock = new Mock<IWalletRepository>();
        _bankService = new BankService(_stockRepositoryMock.Object, _walletRepositoryMock.Object, null!);
    }

    /*
   ============================
       GetBankStateAsync
   ============================
   */

    [Fact]
    public async Task GetBankStateAsync_GetListWithData_ReturnList()
    {
        // Arrange
        var expectedStocks = new List<Stock>
        {
            new Stock { Name = "Intel", BankQuantity = 100 },
            new Stock { Name = "AMD", BankQuantity = 50 }
        };

        _stockRepositoryMock
            .Setup(repo => repo.GetAllStocksAvailableAsync())
            .ReturnsAsync(expectedStocks);

        // Act
        var result = await _bankService.GetBankStateAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Intel", result[0].Name);
    }

    [Fact]
    public async Task GetBankStateAsync_GetListWithNoData_ReturnEmptyList()
    {
        // Arrange
        var emptyList = new List<Stock>();

        _stockRepositoryMock
            .Setup(repo => repo.GetAllStocksAvailableAsync())
            .ReturnsAsync(emptyList);

        // Act
        var result = await _bankService.GetBankStateAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    /*
   ============================
       SetNewBankStateAsync
   ============================
   */

    [Fact]
    public async Task SetNewBankStateAsync_NullArgList_ClearDataAtEnd()
    {
        // Arrange
        List<Stock>? nullList = null;

        // Act
        await _bankService.SetNewBankStateAsync(nullList!);

        // Assert
        _stockRepositoryMock.Verify(repo => repo.ClearAllStocksAsync(), Times.Once);
        _stockRepositoryMock.Verify(repo => repo.AddNewStocksAsync(It.IsAny<List<Stock>>()), Times.Never);
    }

    [Fact]
    public async Task SetNewBankStateAsync_EmptyArgList_ClearDataAtEnd()
    {
        // Arrange
        var emptyList = new List<Stock>();

        // Act
        await _bankService.SetNewBankStateAsync(emptyList);

        // Assert
        _stockRepositoryMock.Verify(repo => repo.ClearAllStocksAsync(), Times.Once);
        _stockRepositoryMock.Verify(repo => repo.AddNewStocksAsync(It.IsAny<List<Stock>>()), Times.Never);
    }

    [Fact]
    public async Task SetNewBankStateAsync_NoEmptyArgList_NormalDataAtEnd()
    {
        // Arrange
        var validStocks = new List<Stock>
        {
            new Stock { Name = "Nvidia", BankQuantity = 10 }
        };

        // Act
        await _bankService.SetNewBankStateAsync(validStocks);

        // Assert
        _stockRepositoryMock.Verify(repo => repo.ClearAllStocksAsync(), Times.Once);

        _stockRepositoryMock.Verify(repo => repo.AddNewStocksAsync(validStocks), Times.Once);
    }

    /*
   ============================
       ProcessBuyRequest
   ============================
   */

    [Fact]
    public async Task ProcessBuyRequest_StockDoesNotExist_Return404()
    {
        // Arrange
        _stockRepositoryMock.Setup(r => r.GetStockByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((Stock)null!);

        // Act
        var result = await _bankService.ProcessBuyRequest("Tesla", 1);

        // Assert
        Assert.Equal(404, result);
    }

    [Fact]
    public async Task ProcessBuyRequest_StockQUantityIs0_Return400()
    {
        // Arrange
        var stock = new Stock { Id = 1, Name = "Tesla", BankQuantity = 0 };
        _stockRepositoryMock.Setup(r => r.GetStockByNameAsync("Tesla")).ReturnsAsync(stock);
        _stockRepositoryMock.Setup(r => r.TryDecreaseStockInBankAtomicAsync(stock)).ReturnsAsync(false);

        // Act
        var result = await _bankService.ProcessBuyRequest("Tesla", 1);

        // Assert
        Assert.Equal(400, result);
    }

    [Fact]
    public async Task ProcessBuyRequest_CreateWalletAndAddStock_Return200()
    {
        // Arrange
        var stock = new Stock { Id = 1, Name = "Tesla", BankQuantity = 10 };
        _stockRepositoryMock.Setup(r => r.GetStockByNameAsync("Tesla")).ReturnsAsync(stock);
        _walletRepositoryMock.Setup(r => r.GetWalletByIdIncludingWalletStocksAsync(1)).ReturnsAsync((Wallet)null!);

        _stockRepositoryMock.Setup(r => r.TryDecreaseStockInBankAtomicAsync(stock)).ReturnsAsync(true);
        _walletRepositoryMock.Setup(r => r.TryIncreaseStockInWalletAtomicAsync(1, stock.Id)).ReturnsAsync(true);

        // Act
        var result = await _bankService.ProcessBuyRequest("Tesla", 1);

        // Assert
        _walletRepositoryMock.Verify(r => r.CreateNewWalletBasedOnIdAsync(1), Times.Once);
        Assert.Equal(200, result);
    }

    [Fact]
    public async Task ProcessBuyRequest_StockAndWalletExistsAddProperly_Return200()
    {
        // Arrange
        var stock = new Stock { Id = 1, Name = "Tesla", BankQuantity = 10 };
        var wallet = new Wallet { Id = 1 };

        _stockRepositoryMock.Setup(r => r.GetStockByNameAsync("Tesla")).ReturnsAsync(stock);
        _walletRepositoryMock.Setup(r => r.GetWalletByIdIncludingWalletStocksAsync(1)).ReturnsAsync(wallet);
        _stockRepositoryMock.Setup(r => r.TryDecreaseStockInBankAtomicAsync(stock)).ReturnsAsync(true);
        _walletRepositoryMock.Setup(r => r.TryIncreaseStockInWalletAtomicAsync(1, stock.Id)).ReturnsAsync(true);

        // Act
        var result = await _bankService.ProcessBuyRequest("Tesla", 1);

        // Assert
        Assert.Equal(200, result);
    }

    [Fact]
    public async Task ProcessBuyRequest_StockNameIsNull_Return404()
    {
        // Act
        var result = await _bankService.ProcessBuyRequest(null!, 1);

        // Assert
        Assert.Equal(404, result);
    }

    [Fact]
    public async Task ProcessBuyRequest_StockNameIsWhiteSpace_Return404()
    {
        // Act
        var result = await _bankService.ProcessBuyRequest("   ", 1);

        // Assert
        Assert.Equal(404, result);
    }

    [Fact]
    public async Task ProcessBuyRequest_BankOkButWalletFails_Return402()
    {
        // Arrange
        var stock = new Stock { Id = 1, Name = "Tesla", BankQuantity = 10 };
        _stockRepositoryMock.Setup(r => r.GetStockByNameAsync("Tesla")).ReturnsAsync(stock);
        _walletRepositoryMock.Setup(r => r.GetWalletByIdIncludingWalletStocksAsync(1)).ReturnsAsync(new Wallet());

        _stockRepositoryMock.Setup(r => r.TryDecreaseStockInBankAtomicAsync(stock)).ReturnsAsync(true);

        _walletRepositoryMock.Setup(r => r.TryIncreaseStockInWalletAtomicAsync(1, stock.Id)).ReturnsAsync(false);

        // Act
        var result = await _bankService.ProcessBuyRequest("Tesla", 1);

        // Assert
        Assert.Equal(402, result);
    }

    /*
   ============================
       ProcessSellRequest
   ============================
   */

    [Fact]
    public async Task ProcessSellRequest_StockNameIsNull_Return404()
    {
        // Act
        var result = await _bankService.ProcessSellRequest(null!, 1);

        // Assert
        Assert.Equal(404, result);
    }

    [Fact]
    public async Task ProcessSellRequest_StockNameIsWhiteSpace_Return404()
    {
        // Act
        var result = await _bankService.ProcessSellRequest("   ", 1);

        // Assert
        Assert.Equal(404, result);
    }

    [Fact]
    public async Task ProcessSellRequest_StockDoesNotExist_Return404()
    {
        // Arrange
        _stockRepositoryMock.Setup(r => r.GetStockByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((Stock)null!);

        // Act
        var result = await _bankService.ProcessSellRequest("Tesla", 1);

        // Assert
        Assert.Equal(404, result);
    }

    [Fact]
    public async Task ProcessSellRequest_WalletDoesNotExist_Return404()
    {
        // Arrange
        var stock = new Stock { Id = 1, Name = "Tesla", BankQuantity = 1 };
        _stockRepositoryMock.Setup(r => r.GetStockByNameAsync("Tesla")).ReturnsAsync(stock);

        _walletRepositoryMock.Setup(r => r.GetWalletByIdIncludingWalletStocksAsync(1))
            .ReturnsAsync((Wallet)null!);

        // Act
        var result = await _bankService.ProcessSellRequest("Tesla", 1);

        // Assert
        Assert.Equal(404, result);
    }

    [Fact]
    public async Task ProcessSellRequest_WalletDoesNotHaveStock_Return400()
    {
        // Arrange
        var stock = new Stock { Id = 1, Name = "Tesla", BankQuantity = 1 };
        var wallet = new Wallet { Id = 1 };

        _stockRepositoryMock.Setup(r => r.GetStockByNameAsync("Tesla")).ReturnsAsync(stock);
        _walletRepositoryMock.Setup(r => r.GetWalletByIdIncludingWalletStocksAsync(1)).ReturnsAsync(wallet);

        _walletRepositoryMock.Setup(r => r.TryDecreaseStockInWalletAtomicAsync(1, stock.Id)).ReturnsAsync(false);

        // Act
        var result = await _bankService.ProcessSellRequest("Tesla", 1);

        // Assert
        Assert.Equal(400, result);
    }

    [Fact]
    public async Task ProcessSellRequest_WalletOkButBankFails_Return402()
    {
        // Arrange
        var stock = new Stock { Id = 1, Name = "Tesla", BankQuantity = 1 };
        var wallet = new Wallet { Id = 1 };

        _stockRepositoryMock.Setup(r => r.GetStockByNameAsync("Tesla")).ReturnsAsync(stock);
        _walletRepositoryMock.Setup(r => r.GetWalletByIdIncludingWalletStocksAsync(1)).ReturnsAsync(wallet);

        _walletRepositoryMock.Setup(r => r.TryDecreaseStockInWalletAtomicAsync(1, stock.Id)).ReturnsAsync(true);
        _stockRepositoryMock.Setup(r => r.TryIncreaseStockInBankAtomicAsync(stock)).ReturnsAsync(false);

        // Act
        var result = await _bankService.ProcessSellRequest("Tesla", 1);

        // Assert
        Assert.Equal(402, result);
    }


    [Fact]
    public async Task ProcessSellRequest_SellProperly_Return200()
    {
        // Arrange
        var stock = new Stock { Id = 1, Name = "Tesla", BankQuantity = 1 };
        var wallet = new Wallet { Id = 1 };

        _stockRepositoryMock.Setup(r => r.GetStockByNameAsync("Tesla")).ReturnsAsync(stock);
        _walletRepositoryMock.Setup(r => r.GetWalletByIdIncludingWalletStocksAsync(1)).ReturnsAsync(wallet);

        _walletRepositoryMock.Setup(r => r.TryDecreaseStockInWalletAtomicAsync(1, stock.Id)).ReturnsAsync(true);
        _stockRepositoryMock.Setup(r => r.TryIncreaseStockInBankAtomicAsync(stock)).ReturnsAsync(true);

        // Act
        var result = await _bankService.ProcessSellRequest("Tesla", 1);

        // Assert
        Assert.Equal(200, result);
    }

    /*
  ============================
      GetStocksQuantityInWalletAsync
  ============================
  */

    [Fact]
    public async Task GetStocksQuantityInWalletAsync_NegativeNumberId_ReturnMinus1()
    {
        // Arrange
        _walletRepositoryMock.Setup(r => r.GetWalletByIdIncludingWalletStocksAsync(-1))
            .ReturnsAsync((Wallet)null!);

        _walletRepositoryMock.Setup(r => r.GetStockQuantityInWalletByIdAsync((Wallet?)null, It.IsAny<string>()))
            .ReturnsAsync(-1);

        // Act
        var result = await _bankService.GetStocksQuantityInWalletAsync("Intel", -1);

        // Assert
        Assert.Equal(-1, result);
    }

    [Fact]
    public async Task GetStocksQuantityInWalletAsync_IdIsZero_ReturnMinus1()
    {
        // Arrange
        _walletRepositoryMock.Setup(r => r.GetWalletByIdIncludingWalletStocksAsync(0))
            .ReturnsAsync((Wallet)null!);

        _walletRepositoryMock.Setup(r => r.GetStockQuantityInWalletByIdAsync((Wallet?)null, It.IsAny<string>()))
            .ReturnsAsync(-1);

        // Act
        var result = await _bankService.GetStocksQuantityInWalletAsync("Intel", 0);

        // Assert
        Assert.Equal(-1, result);
    }

    [Fact]
    public async Task GetStocksQuantityInWalletAsync_StringArgumentIsWhitespace_ReturnMinus1()
    {
        // Arrange
        var wallet = new Wallet { Id = 1 };
        _walletRepositoryMock.Setup(r => r.GetWalletByIdIncludingWalletStocksAsync(1)).ReturnsAsync(wallet);
        _walletRepositoryMock.Setup(r => r.GetStockQuantityInWalletByIdAsync(wallet, "   ")).ReturnsAsync(-1);

        // Act
        var result = await _bankService.GetStocksQuantityInWalletAsync("   ", 1);

        // Assert
        Assert.Equal(-1, result);
    }

    [Fact]
    public async Task GetStocksQuantityInWalletAsync_StringArgumentIsNull_ReturnMinus1()
    {
        // Arrange
        var wallet = new Wallet { Id = 1 };
        _walletRepositoryMock.Setup(r => r.GetWalletByIdIncludingWalletStocksAsync(1)).ReturnsAsync(wallet);
        _walletRepositoryMock.Setup(r => r.GetStockQuantityInWalletByIdAsync(wallet, null!)).ReturnsAsync(-1);

        // Act
        var result = await _bankService.GetStocksQuantityInWalletAsync(null!, 1);

        // Assert
        Assert.Equal(-1, result);
    }

    [Fact]
    public async Task GetStocksQuantityInWalletAsync_StockExistsAndWalletExists_ReturnQuantity()
    {
        // Arrange
        var wallet = new Wallet { Id = 1 };
        _walletRepositoryMock.Setup(r => r.GetWalletByIdIncludingWalletStocksAsync(1)).ReturnsAsync(wallet);
        _walletRepositoryMock.Setup(r => r.GetStockQuantityInWalletByIdAsync(wallet, "Intel")).ReturnsAsync(7);

        // Act
        var result = await _bankService.GetStocksQuantityInWalletAsync("Intel", 1);

        // Assert
        Assert.Equal(7, result);
    }

    /*
   ============================
       GetWalletsCurrentStateAsync
   ============================
   */

    [Fact]
    public async Task GetWalletsCurrentStateAsync_IdIsZero_ReturnEmptyList()
    {
        // Act
        var result = await _bankService.GetWalletsCurrentStateAsync(0);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetWalletsCurrentStateAsync_IdIsLowerThanZero_ReturnEmptyList()
    {
        // Act
        var result = await _bankService.GetWalletsCurrentStateAsync(-5);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetWalletsCurrentStateAsync_IdIsValidAndWalletExists_ReturnList()
    {
        // Arrange
        var wallet = new Wallet { Id = 1 };
        wallet.WalletStocks.Add(new WalletStock
        {
            WalletId = 1,
            StockId = 2,
            Quantity = 3,
            Wallet = wallet,
            Stock = new Stock { Id = 2, Name = "Intel", BankQuantity = 1 }
        });
        _walletRepositoryMock.Setup(r => r.GetWalletByIdIncludingWalletStocksAsync(1)).ReturnsAsync(wallet);

        // Act
        var result = await _bankService.GetWalletsCurrentStateAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(3, result[0].Quantity);
    }

    [Fact]
    public async Task GetWalletsCurrentStateAsync_IdIsValidAndWalletNotExists_ReturnEmptyList()
    {
        // Arrange
        _walletRepositoryMock.Setup(r => r.GetWalletByIdIncludingWalletStocksAsync(1)).ReturnsAsync((Wallet)null!);

        // Act
        var result = await _bankService.GetWalletsCurrentStateAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}