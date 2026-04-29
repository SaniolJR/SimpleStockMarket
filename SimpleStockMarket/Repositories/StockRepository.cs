using Database;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Services;

public class StockRepository : IStockRepository
{
    private readonly MainDbContext _db;

    public StockRepository(MainDbContext db) => _db = db;

    public async Task<Stock?> GetByNameAsync(string name) 
        => await _db.Stocks.FirstOrDefaultAsync(s => s.Name == name);

    public async Task<Stock?> GetByIdAsync(int id) 
        => await _db.Stocks.FirstOrDefaultAsync(s => s.Id == id);
}
