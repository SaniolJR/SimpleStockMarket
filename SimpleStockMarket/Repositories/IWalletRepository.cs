using Entities;

namespace Services;

public interface IWalletRepository
{
    Task<Wallet?> GetWalletByIdIncludingWalletStocksAsync(int id);
    Task<int> GetStockQuantityInWalletByIdAsync(Wallet? wallet, string stockName);
    Task<bool> TryDecreaseStockInWalletAtomicAsync(int walletId, int stockId);
    Task<bool> TryIncreaseStockInWalletAtomicAsync(int walletId, int stockId);
    Task CreateNewWalletBasedOnIdAsync(int id);
}
