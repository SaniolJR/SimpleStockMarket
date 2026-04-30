using Microsoft.AspNetCore.Mvc;
using Services;
using DTOs;
using Mappers;

namespace Controllers;

[ApiController]
[Route("stocks")]
public class StocksControllers : ControllerBase
{
    private readonly IBankService _BankService;

    public StocksControllers(IBankService BankService)
    {
        _BankService = BankService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCurrentStateOfBank()
    {
        var bankState = await _BankService.GetBankStateAsync();

        //Parse to the right format
        var ans = StockMapper.StockListToDTO(bankState);

        return Ok(ans);
    }

    [HttpPost]
    public async Task<IActionResult> SetNewStateOfBank([FromBody] StocksListDTO dto)
    {
        var stockList = StockMapper.StocksListDtoToEntity(dto);
        try
        {
            //if dto was null and stockList is null - just delete bank data
            await _BankService.SetNewBankStateAsync(stockList);
            return Ok();
        }
        catch(Exception ex)
        {
            return StatusCode(500,"Error durning setting new state of bank.");
        }

    }
}