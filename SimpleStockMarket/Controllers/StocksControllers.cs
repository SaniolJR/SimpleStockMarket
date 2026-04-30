using Microsoft.AspNetCore.Mvc;
using Services;
using DTOs;

namespace Controllers;

[ApiController]
[Route("stocks")]
public class StocksControllers : ControllerBase
{
    private readonly IBankService _BankService;

    public StocksControllers(IBankService BankServicee)
    {
        _BankService = BankServicee;
    }

    [HttpGet]
    public async Task<IActionResult> GetCurrentStateOfBank()
    {
        var bankState = await _BankService.GetBankStateAsync();

        //Parse to the right format
        var ans = new
        {
            Stocks = bankState.Select(s => new
            {
                s.Name,
                Quantity = s.BankQuantity
            })
        };

        return Ok(ans);
    }

    [HttpPost]
    public async Task<IActionResult> SetNewStateOfBank([FromBody] StocksListDTO dto)
    {
        //TODO: mapping
        return Ok();
    }
}