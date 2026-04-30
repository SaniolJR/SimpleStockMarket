using Entities;
using DTOs;

namespace Mappers;

public class StockMapper
{
    
    public static Stock? StockDtoToEntity(StockItemDTO? stockDTO)
    {
        if (stockDTO == null || string.IsNullOrWhiteSpace(stockDTO.Name))
            return null;

        return new Stock { Name = stockDTO.Name, BankQuantity = stockDTO.Quantity };
    }

    public static List<Stock>? StocksListDtoToEntity(StocksListDTO? dto)
    {
        if (dto == null)
            return null;

        return dto.Stocks?
            .Select(StockDtoToEntity)
            .Where(stock => stock != null)
            .Cast<Stock>()
            .ToList() ?? new List<Stock>();
    }

    public static StockItemDTO? StockToDTO(Stock? stock)
    {
        if (stock == null) return null;

        return new StockItemDTO 
        { 
            Name = stock.Name, 
            Quantity = stock.BankQuantity 
        };
    }

    public static StocksListDTO StockListToDTO(List<Stock>? list)
    {
        return new StocksListDTO
        {
            Stocks = list?.Select(StockToDTO)
                        .Where(x => x != null)
                        .Cast<StockItemDTO>()
                        .ToList() ?? new List<StockItemDTO>()
        };
    }
}