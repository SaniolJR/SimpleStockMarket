using Entities;
using Microsoft.IdentityModel.Tokens;

namespace Services;

public class BankService : IBankService
{
    private readonly IStockRepository _stockRepository;

    public BankService(IStockRepository stockRepository)
    {
        _stockRepository = stockRepository;
    }

    public Task ProcessBuyRequest(string name, int walletID)
    {
        //wydobyj obiekt akcji
        //wydobyj obiekt portfela
        
        //sprawdz czy akcja jest dostepna do kupna
        //
        return null;
    }

    public Task ProcessSellRequest()
    {
        return null;
    }

    public async Task<List<Stock>> GetBankStateAsync()
    {
        var bankState = new List<Stock>();

        var stocksToAdd = await _stockRepository.GetAllStocksAvailableAsync();
        bankState.AddRange(stocksToAdd);

        return bankState;
    }

    public async Task SetNewBankStateAsync(List<Stock> stocks)
    {
        //Clear whole current data of stocks

        await _stockRepository.ClearAllStocksAsync();

        //If List is empty just clear whole database
        if (!stocks.IsNullOrEmpty())
        {
            await _stockRepository.AddNewStocksAsync(stocks);
        }
    }

}