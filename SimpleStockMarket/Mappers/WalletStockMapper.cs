using Entities;
using DTOs;

namespace Mappers;

public record WalletDetailsDTO(
    string id,
    List<StockInfoDTO> stocks
);

public record StockInfoDTO(
    string name,
    int quantity
);
public static class WalletStockMapper
{
    public static WalletDetailsDTO MapToDetailsDTO(List<WalletStock> walletStocks, int walletId)
    {
        return new WalletDetailsDTO(
            id: walletId.ToString(),
            stocks: walletStocks.Select(ws => new StockInfoDTO(
                name: ws.Stock.Name,
                quantity: ws.Quantity
            )).ToList()
        );
    }
}