using Entities;

namespace Services;

public interface IWalletRepository
{
    Task<Wallet?> GetWalletByIdAsync(int id);
}
