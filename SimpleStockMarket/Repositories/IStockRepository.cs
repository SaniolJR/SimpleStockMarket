using Entities;

namespace Services;

public interface IStockRepository
{
    Task<Stock?> GetStockByNameAsync(string name);

    Task<List<Stock>?> GetAllStocksAvailableAsync();

    Task AddNewStocksAsync(List<Stock> stocks);
    Task ClearAllStocksAsync();
}
