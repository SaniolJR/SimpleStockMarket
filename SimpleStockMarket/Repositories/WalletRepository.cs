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
        return null;
    }

}
