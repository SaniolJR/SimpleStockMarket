using Services;
using Xunit;
using Moq;
using Entities;

namespace Tests.ServicesTests;

public class BankServiceTests
{
    private (BankService Service, Mock<IStockRepository> Repository) CreateServiceWithRepository()
    {
        var mockRepo = new Mock<IStockRepository>();
        var service = new BankService(mockRepo.Object);
        return (service, mockRepo);
    }

    [Fact]
    public async Task GetStockByName_StockExists_Positive()
    {
        // Arrange
        var (service, mockRepo) = CreateServiceWithRepository();
        var stock = new Stock { Id = 1, Name = "Nvidia", BankQuantity = 100 };
        mockRepo.Setup(r => r.GetByNameAsync("Nvidia")).ReturnsAsync(stock);

        // Act
        var result = await service.GetStockByName("Nvidia");

        // Assert
        Assert.Equal("Nvidia", result.Name);
        Assert.Equal(100, result.BankQuantity);
    }

    [Fact]
    public async Task GetStockByName_StockDontExists_Negative()
    {
        // Arrange
        var (service, mockRepo) = CreateServiceWithRepository();
        mockRepo.Setup(r => r.GetByNameAsync(It.IsAny<string>())).ReturnsAsync((Stock?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetStockByName("AMD"));
    }

    [Fact]
    public async Task GetStockByName_StockArgumentIsNull_Negative()
    {
        // Arrange
        var (service, mockRepo) = CreateServiceWithRepository();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.GetStockByName(null!));
    }

    [Fact]
    public async Task GetStockByName_StockArgumentIsWhiteSpace_Negative()
    {
        // Arrange
        var (service, mockRepo) = CreateServiceWithRepository();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.GetStockByName(" "));
    }
}