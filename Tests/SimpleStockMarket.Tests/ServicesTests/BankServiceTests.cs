using Moq;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using Services; 
using Entities; 

public class BankServiceTests
{
    private readonly Mock<IStockRepository> _stockRepositoryMock;
    private readonly BankService _bankService;

    public BankServiceTests()
    {
        _stockRepositoryMock = new Mock<IStockRepository>();
        _bankService = new BankService(_stockRepositoryMock.Object);
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
        Assert.Empty(result); // Sprawdza czy lista ma 0 elementów
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
        // Sprawdzamy, czy AddNewStocksAsync NIE zostało wywołane
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
        // Upewniamy się, że najpierw wyczyszczono stare dane
        _stockRepositoryMock.Verify(repo => repo.ClearAllStocksAsync(), Times.Once);
        
        // Następnie upewniamy się, że dodano nowe
        _stockRepositoryMock.Verify(repo => repo.AddNewStocksAsync(validStocks), Times.Once);
    }

}