using Entities;

namespace Services;

public interface IWalletRepository
{
    Task<Wallet?> GetWalletByIdAsync(int id);
    Task<int> GetStockQuantityInWalletByIdAsync(Wallet wallet, string stockName);
    Task<bool> TryDecreaseStockInWalletAtomicAsync(Wallet wallet);
    Task<bool> TryIncreaseStockInWalletAtomicAsync(Wallet wallet);
}
