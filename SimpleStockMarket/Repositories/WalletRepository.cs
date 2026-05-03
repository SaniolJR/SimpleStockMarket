using Database;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Services;

public class WalletRepository : IWalletRepository
{
    private readonly MainDbContext _db;

    public WalletRepository(MainDbContext db) => _db = db;

    public async Task<Wallet?> GetWalletByIdAsync(int id)
    {
        if (id <= 0)
        {
            return null;
        }
        return await _db.Wallets.FirstOrDefaultAsync(s => s.Id == id);
    }
    public async Task<int> GetStockQuantityInWalletByIdAsync(Wallet wallet, string stockName)
    {
        if (wallet == null) return -1;
        if (string.IsNullOrWhiteSpace(stockName)) return -1;

        var walletStock = wallet.Stocks.FirstOrDefault(s => s.Stock.Name == stockName);

        if (walletStock == null) return -1;
        return walletStock.Quantity;
    }
    public async Task<bool> TryDecreaseStockInWalletAtomicAsync(Wallet wallet)
    {
        return true;
    }
    public async Task<bool> TryIncreaseStockInWalletAtomicAsync(Wallet wallet)
    {
        return true;
    }

}
