using Entities;

namespace Services;

public interface IBankService
{
    public Task ProcessBuyRequest(string name, int walletID);

    public Task ProcessSellRequest();

    public Task GetBankState();

    public Task SetNewBankState();
}