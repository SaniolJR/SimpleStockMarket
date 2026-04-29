using Entities;

namespace Services;

public class BankService : IBankService
{
    private readonly IStockRepository _stockRepository;

    public BankService(IStockRepository stockRepository)
    {
        _stockRepository = stockRepository;
    }

    public async Task<Stock> GetStockByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Stock name was not given.");

        var stock = await _stockRepository.GetByNameAsync(name);
        if (stock == null) throw new InvalidOperationException($"Stock '{name}' not found.");
        return stock;
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

    public Task GetBankState()
    {
        return null;
    }

    public Task SetNewBankState()
    {
        return null;
    }

}