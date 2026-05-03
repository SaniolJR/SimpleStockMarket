using Entities;

namespace Services;

public interface IBankService
{
    public Task<int> ProcessBuyRequest(string name, int walletID);

    public Task<int> ProcessSellRequest(string name, int walletID);

    public Task<List<Stock>> GetBankStateAsync();

    public Task SetNewBankStateAsync(List<Stock>? stocks);
    public Task<int> GetStocksQuantityInWalletAsync(string name, int walletID);

    public Task<List<WalletStock>> GetWalletsCurrentStateAsync(int walletID);
}