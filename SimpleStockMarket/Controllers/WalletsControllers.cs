using Microsoft.AspNetCore.Mvc;
using Services;
using DTOs;
using Mappers;
namespace Controllers;

[ApiController]
[Route("wallets")]
public class WalletsControllers : ControllerBase
{
    private readonly IBankService _BankService;

    public WalletsControllers(IBankService BankService)
    {
        _BankService = BankService;
    }

    [HttpPost("{walletId}/stocks/{stockName}")]
    public async Task<IActionResult> HandleStockTransaction(
        [FromRoute] StockAndWalletDTO stockAndWalletDTO,
        [FromBody] TransactionRequestDTO requestDTO)
    {
        int statusCode;

        if (requestDTO.Type == "buy")
        {
            statusCode = await _BankService.ProcessBuyRequest(stockAndWalletDTO.stockName, stockAndWalletDTO.walletID);
        }
        else if (requestDTO.Type == "sell")
        {
            statusCode = await _BankService.ProcessSellRequest(stockAndWalletDTO.stockName, stockAndWalletDTO.walletID);
        }
        else
        {
            return BadRequest("Invalid transaction type.");
        }

        return statusCode switch
        {
            200 => Ok(),
            400 => BadRequest(),
            404 => NotFound(),
            402 => StatusCode(402),
            _ => StatusCode(500)
        };
    }

    [HttpGet("{walletId}/stocks/{stockName}")]
    public async Task<IActionResult> GetQuantityOfStockInWallet(
                [FromRoute] StockAndWalletDTO stockAndWalletDTO)
    {
        int stockQuantity = await _BankService.
                        GetStocksQuantityInWalletAsync(stockAndWalletDTO.stockName, stockAndWalletDTO.walletID);
        if (stockQuantity == -1)
        {
            return BadRequest();
        }

        return Ok(stockQuantity);
    }

    [HttpGet("{walletId}")]
    public async Task<IActionResult> GetCurrentStateOfWallet(
                [FromRoute] WalletDTO walletDTO)
    {
        var currentState = await _BankService.GetWalletsCurrentStateAsync(walletDTO.walletID);
        var result = WalletStockMapper.MapToDetailsDTO(currentState, walletDTO.walletID);

        return Ok(result);
    }

}