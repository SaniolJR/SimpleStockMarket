using Entities;

namespace Services;

public interface ILogRepository
{
    public Task CreateAndLogTransactionAsync(Wallet wallet, Stock stock, TransactionType type);
    public Task<List<AuditLog>> GetAllLogs();
}
