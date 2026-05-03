using Database;
using Entities;
using Microsoft.EntityFrameworkCore;
using EFCore.BulkExtensions;

namespace Services;

public class LogRepository : ILogRepository
{
    private readonly MainDbContext _db;

    public LogRepository(MainDbContext db) => _db = db;
    public async Task CreateAndLogTransactionAsync(Wallet wallet, Stock stock, TransactionType type)
    {
        var auditLog = new AuditLog
        {
            Type = type,
            UsedWallet = wallet,
            UsedStock = stock,
            TransactionDate = DateTime.UtcNow
        };

        await _db.AuditLogs.AddAsync(auditLog);

        var result = await _db.SaveChangesAsync();
    }

    public async Task<List<AuditLog>> GetAllLogs()
    {
        return await _db.AuditLogs
        .AsNoTracking()
        .Include(l => l.UsedWallet)
        .Include(l => l.UsedStock)
        .ToListAsync();
    }
}