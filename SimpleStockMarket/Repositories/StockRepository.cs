using Database;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Services;

public class StockRepository : IStockRepository
{
    private readonly MainDbContext _db;

    public StockRepository(MainDbContext db) => _db = db;

    public async Task<Stock?> GetStockByNameAsync(string name)
    {
          return await _db.Stocks.FirstOrDefaultAsync(s => s.Name == name);
    }

    public async Task<List<Stock>?> GetAllStocksAvailableAsync(){

    }
}
