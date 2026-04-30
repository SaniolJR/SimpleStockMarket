using Entities;

namespace Services;

public interface IStockRepository
{
    Task<Stock?> GetStockByNameAsync(string name);
}
