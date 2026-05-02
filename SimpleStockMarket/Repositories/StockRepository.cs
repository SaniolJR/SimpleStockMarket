using Database;
using Entities;
using Microsoft.EntityFrameworkCore;
using EFCore.BulkExtensions;

namespace Services;

public class StockRepository : IStockRepository
{
    private readonly MainDbContext _db;

    public StockRepository(MainDbContext db) => _db = db;

    public async Task<Stock?> GetStockByNameAsync(string name)
    {
        return await _db.Stocks.FirstOrDefaultAsync(s => s.Name == name);
    }

    public async Task<List<Stock>?> GetAllStocksAvailableAsync()
    {
        return await _db.Stocks.ToListAsync();
    }

    public async Task AddNewStocksAsync(List<Stock> stocks)
    {
        await _db.BulkInsertAsync(stocks);
    }

    public async Task ClearAllStocksAsync()
    {
        await _db.Stocks.ExecuteDeleteAsync();
    }

    public async Task<bool> TryDecreaseStockInBankAtomicAsync(Stock stock)
    {
        int rowsAffected = await _context.Stocks
        .Where(s => s.Id == stock.Id && s.BankQuantity > 0)
        .ExecuteUpdateAsync(s => s.SetProperty(b => b.BankQuantity, b => b.BankQuantity - 1));

        if (rowsAffected == 0)
        {
            return false;
        }
        return true;
    }
    public async Task<bool> TryIncreaseStockInBankAtomicAsync(Stock stock)
    {
        int rowsAffected = await _context.Stocks
        .Where(s => s.Id == stock.Id)
        .ExecuteUpdateAsync(s => s.SetProperty(b => b.BankQuantity, b => b.BankQuantity + 1));

        if (rowsAffected == 0)
        {
            return false;
        }
        return true;
    }
}
