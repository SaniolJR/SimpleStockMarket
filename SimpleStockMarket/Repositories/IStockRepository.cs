using Entities;

namespace Services;

public interface IStockRepository
{
    Task<Stock?> GetByNameAsync(string name);
    Task<Stock?> GetByIdAsync(int id);
}
