using System.Data.Common;
using Entities;
using Microsoft.IdentityModel.Tokens;
using Database;

namespace Services;

public class BankService : IBankService
{
    private readonly IStockRepository _stockRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly ILogRepository _logRepository;
    private readonly MainDbContext? _db;

    public BankService(IStockRepository stockRepository, IWalletRepository walletRepository, ILogRepository logRepository, MainDbContext? db)
    {
        _stockRepository = stockRepository;
        _walletRepository = walletRepository;
        _logRepository = logRepository;
        _db = db;
    }

    public async Task<int> ProcessBuyRequest(string name, int walletID)
    {
        if (string.IsNullOrWhiteSpace(name))
            return 404;

        // Open transaction for possible rollback when a DbContext is available.
        var transaction = _db != null ? await _db.Database.BeginTransactionAsync() : null;

        try
        {
            //get stock from db
            var stock = await _stockRepository.GetStockByNameAsync(name);
            if (stock == null)
                return 404;

            //get wallet from db, and eventually create new one
            var wallet = await _walletRepository.GetWalletByIdIncludingWalletStocksAsync(walletID);
            if (wallet == null)
            {
                await _walletRepository.CreateNewWalletBasedOnIdAsync(walletID);
                wallet = await _walletRepository.GetWalletByIdIncludingWalletStocksAsync(walletID);
                if (wallet == null) return 500;
            }

            // try decrease stock quantity in bank
            bool decreaseStock = await _stockRepository.TryDecreaseStockInBankAtomicAsync(stock);
            if (!decreaseStock)
                return 400; //probably quantity is too low

            // add to stock wallet
            bool increaseStock = await _walletRepository.TryIncreaseStockInWalletAtomicAsync(walletID, stock.Id);

            if (!increaseStock)
            {
                // wallet declined - rollback needed 
                if (transaction is not null)
                    await transaction.RollbackAsync();
                return 402; //for decline error
            }

            //commit changes of transaction
            if (transaction != null)
                await transaction.CommitAsync();

            //success make log
            var transactionType = TransactionType.buy;
            await _logRepository.CreateAndLogTransactionAsync(wallet, stock, transactionType);
            return 200;
        }
        catch (Exception)
        {
            if (transaction is not null)
                await transaction!.RollbackAsync();
            throw;
        }
        finally
        {
            if (transaction != null)
                await transaction.DisposeAsync();
        }
    }

    public async Task<int> ProcessSellRequest(string name, int walletID)
    {
        if (string.IsNullOrWhiteSpace(name))
            return 404;

        // Open transaction for possible rollback when a DbContext is available
        var transaction = _db != null ? await _db.Database.BeginTransactionAsync() : null;

        try
        {
            //get stock from db
            var stock = await _stockRepository.GetStockByNameAsync(name);
            if (stock == null)
                return 404;

            //get wallet from db, and eventually create new one
            var wallet = await _walletRepository.GetWalletByIdIncludingWalletStocksAsync(walletID);
            if (wallet == null)
                return 404;

            // try decrease stock quantity in bank
            bool decreaseStock = await _walletRepository.TryDecreaseStockInWalletAtomicAsync(walletID, stock.Id);
            if (!decreaseStock)
                return 400; //probably quantity is too low

            // add to stock wallet
            bool increaseStock = await _stockRepository.TryIncreaseStockInBankAtomicAsync(stock);

            if (!increaseStock)
            {
                // wallet declined - rollback needed 
                if (transaction is not null)
                    await transaction.RollbackAsync();
                return 402; //for decline error
            }

            //commit changes of transaction
            if (transaction != null)
                await transaction.CommitAsync();

            //success make log
            var transactionType = TransactionType.sell;
            await _logRepository.CreateAndLogTransactionAsync(wallet, stock, transactionType);
            return 200;
        }
        catch (Exception)
        {
            if (transaction is not null)
                await transaction!.RollbackAsync();
            throw;
        }
        finally
        {
            if (transaction != null)
                await transaction.DisposeAsync();
        }
    }

    public async Task<List<Stock>> GetBankStateAsync()
    {
        var bankState = new List<Stock>();

        var stocksToAdd = await _stockRepository.GetAllStocksAvailableAsync();
        if (stocksToAdd != null)
            bankState.AddRange(stocksToAdd);

        return bankState;
    }

    public async Task SetNewBankStateAsync(List<Stock>? stocks)
    {
        //Clear whole current data of stocks

        await _stockRepository.ClearAllStocksAsync();

        //If List is empty just clear whole database
        if (stocks != null && !stocks.IsNullOrEmpty())
        {
            await _stockRepository.AddNewStocksAsync(stocks);
        }
    }

    public async Task<int> GetStocksQuantityInWalletAsync(string name, int walletID)
    {
        var wallet = await _walletRepository.GetWalletByIdIncludingWalletStocksAsync(walletID);
        return await _walletRepository.GetStockQuantityInWalletByIdAsync(wallet, name);
    }

    public async Task<List<WalletStock>> GetWalletsCurrentStateAsync(int walletID)
    {
        List<WalletStock> currentState = new List<WalletStock>();
        if (walletID <= 0)
            return currentState;

        var wallet = await _walletRepository.GetWalletByIdIncludingWalletStocksAsync(walletID);

        if (wallet != null)
            currentState = wallet.WalletStocks;

        return currentState;

    }

    public async Task<List<AuditLog>> GetBankAuditLog()
    {
        var auditLogs = await _logRepository.GetAllLogs();
        return auditLogs.OrderBy(l => l.TransactionDate).ToList();
    }
}