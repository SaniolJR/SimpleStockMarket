using DTOs;
using Entities;
using Mappers;

namespace Tests.MappersTests;

public class StockMapperTests
{

    /*
    ============================
        StockDtoToEntity
    ============================
    */

    [Fact]
    public void StockDtoToEntity_ArgumentIsNull_ReturnNull()
    {
        var result = StockMapper.StockDtoToEntity(null);

        Assert.Null(result);
    }

    [Fact]
    public void StockDtoToEntity_ConvertProperly_ReturnDTO()
    {
        var dto = new StockItemDTO { Name = "Nvidia", Quantity = 15 };

        var result = StockMapper.StockDtoToEntity(dto);

        Assert.NotNull(result);
        Assert.Equal("Nvidia", result!.Name);
        Assert.Equal(15, result.BankQuantity);
    }

    [Fact]
    public void StockDtoToEntity_DtosNameIsWhitespace_ReturnNull()
    {
        var dto = new StockItemDTO { Name = "   ", Quantity = 15 };

        var result = StockMapper.StockDtoToEntity(dto);

        Assert.Null(result);
    }

    [Fact]
    public void StockDtoToEntity_DtosNameIsNull_ReturnNull()
    {
        var dto = new StockItemDTO { Name = null!, Quantity = 15 };

        var result = StockMapper.StockDtoToEntity(dto);

        Assert.Null(result);
    }

    /*
    ============================
        StocksListDtoToEntity
    ============================
    */

    [Fact]
    public void StocksListDtoToEntity_DtoIsNull_ReturnNull()
    {
        var result = StockMapper.StocksListDtoToEntity(null);

        Assert.Null(result);
    }

    [Fact]
    public void StocksListDtoToEntity_SomeStocksInDtoAreNull_ReturnOnlyNotNull()
    {
        var dto = new StocksListDTO
        {
            Stocks =
            [
                new StockItemDTO { Name = "Nvidia", Quantity = 10 },
                null!,
                new StockItemDTO { Name = "AMD", Quantity = 5 }
            ]
        };

        var result = StockMapper.StocksListDtoToEntity(dto);

        Assert.NotNull(result);
        Assert.Equal(2, result!.Count);
        Assert.Equal("Nvidia", result[0].Name);
        Assert.Equal("AMD", result[1].Name);
    }

    [Fact]
    public void StocksListDtoToEntity_SomeStocksInDtoHaveInvalidName_ReturnOnlyValid()
    {
        var dto = new StocksListDTO
        {
            Stocks =
            [
                new StockItemDTO { Name = "Nvidia", Quantity = 10 },
                new StockItemDTO { Name = "   ", Quantity = 7 },
                new StockItemDTO { Name = null!, Quantity = 3 },
                new StockItemDTO { Name = "AMD", Quantity = 5 }
            ]
        };

        var result = StockMapper.StocksListDtoToEntity(dto);

        Assert.NotNull(result);
        Assert.Equal(2, result!.Count);
        Assert.All(result, stock => Assert.False(string.IsNullOrWhiteSpace(stock.Name)));
    }

    [Fact]
    public void StocksListDtoToEntity_AllStocksAreValid_ReturnStocksList()
    {
        var dto = new StocksListDTO
        {
            Stocks =
            [
                new StockItemDTO { Name = "Nvidia", Quantity = 10 },
                new StockItemDTO { Name = "AMD", Quantity = 5 }
            ]
        };

        var result = StockMapper.StocksListDtoToEntity(dto);

        Assert.NotNull(result);
        Assert.Equal(2, result!.Count);
        Assert.Equal(10, result[0].BankQuantity);
        Assert.Equal(5, result[1].BankQuantity);
    }

    /*
    ============================
        StockToDTO
    ============================
    */

    [Fact]
    public void StockToDTO_StockIsNull_ReturnNull()
    {
        var result = StockMapper.StockToDTO(null);

        Assert.Null(result);
    }

    [Fact]
    public void StockToDTO_StockIsProper_ReturnDto()
    {
        var stock = new Stock { Name = "Nvidia", BankQuantity = 15 };

        var result = StockMapper.StockToDTO(stock);

        Assert.NotNull(result);
        Assert.Equal("Nvidia", result!.Name);
        Assert.Equal(15, result.Quantity);
    }

    [Fact]
    public void StockListToDTO_StockListIsNull_ReturnEmpty()
    {
        var result = StockMapper.StockListToDTO(null);

        Assert.NotNull(result);
        Assert.Empty(result.Stocks);
    }

    [Fact]
    public void StockListToDTO_StockListHaveNulls_ReturnOnlyValid()
    {
        var stocks = new List<Stock?>
        {
            new Stock { Name = "Nvidia", BankQuantity = 10 },
            null,
            new Stock { Name = "AMD", BankQuantity = 5 }
        };

        var result = StockMapper.StockListToDTO(stocks!);

        Assert.NotNull(result);
        Assert.Equal(2, result.Stocks.Count);
        Assert.Equal("Nvidia", result.Stocks[0].Name);
        Assert.Equal("AMD", result.Stocks[1].Name);
    }

    [Fact]
    public void StockListToDTO_AllStockListIsValid_ReturnFullDTO()
    {
        var stocks = new List<Stock>
        {
            new Stock { Name = "Nvidia", BankQuantity = 10 },
            new Stock { Name = "AMD", BankQuantity = 5 }
        };

        var result = StockMapper.StockListToDTO(stocks);

        Assert.NotNull(result);
        Assert.Equal(2, result.Stocks.Count);
        Assert.Equal(10, result.Stocks[0].Quantity);
        Assert.Equal(5, result.Stocks[1].Quantity);
    }
}