using Entities;

namespace Services;

public interface IBankService
{
    public Task ProcessBuyRequest(string name, int walletID);

    public Task ProcessSellRequest();

    public Task<List<Stock>> GetBankStateAsync();

    public Task SetNewBankStateAsync(List<Stock>? stocks);
}