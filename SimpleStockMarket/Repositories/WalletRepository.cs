using Database;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Services;

public class WalletRepository : IWalletRepository
{
    private readonly MainDbContext _db;

    public WalletRepository(MainDbContext db) => _db = db;

    public async Task<Wallet?> GetWalletByIdIncludingWalletStocksAsync(int id)
    {
        if (id <= 0)
        {
            return null;
        }
        return await _db.Wallets
                .Include(w => w.WalletStocks)
                .FirstOrDefaultAsync(w => w.Id == id);
    }
    public async Task<int> GetStockQuantityInWalletByIdAsync(Wallet wallet, string stockName)
    {
        if (wallet == null) return -1;
        if (string.IsNullOrWhiteSpace(stockName)) return -1;

        var walletStock = wallet.WalletStocks.FirstOrDefault(s => s.Stock.Name == stockName);

        if (walletStock == null) return -1;
        return walletStock.Quantity;
    }
    public async Task<bool> TryDecreaseStockInWalletAtomicAsync(int walletId, int stockId)
    {
        if (walletId <= 0)
            throw new ArgumentException("Wallet IF should be > 0.", nameof(walletId));

        if (stockId <= 0)
            throw new ArgumentException("Stock ID should be > 0.", nameof(stockId));

        int rowsAffected = await _db.WalletStocks
            .Where(ws => ws.WalletId == walletId
                      && ws.StockId == stockId
                      && ws.Quantity > 0)
            .ExecuteUpdateAsync(s => s.SetProperty(ws => ws.Quantity, ws => ws.Quantity - 1));

        return rowsAffected > 0;
    }
    public async Task<bool> TryIncreaseStockInWalletAtomicAsync(int walletId, int stockId)
    {
        if (walletId <= 0)
            throw new ArgumentException("Wallet ID should be > 0.", nameof(walletId));

        if (stockId <= 0)
            throw new ArgumentException("Stock ID should be > 0.", nameof(stockId));

        int rowsAffected = await _db.WalletStocks
        .Where(ws => ws.WalletId == walletId
                && ws.StockId == stockId)
        .ExecuteUpdateAsync(s => s.SetProperty(ws => ws.Quantity, ws => ws.Quantity + 1));

        return rowsAffected > 0;

    }

    public async Task CreateNewWalletBasedOnIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Wallet ID should be > 0.");

        Wallet wallet = new Wallet { Id = id };

        await _db.Wallets.AddAsync(wallet);
        await _db.SaveChangesAsync();
    }

}
