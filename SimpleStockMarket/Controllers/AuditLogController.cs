using Microsoft.AspNetCore.Mvc;
using Services;
using DTOs;
using Mappers;
namespace Controllers;

[ApiController]
public class AuditLogController : ControllerBase
{
    private readonly IBankService _BankService;

    public AuditLogController(IBankService BankService)
    {
        _BankService = BankService;
    }

    [HttpGet("log")]
    public async Task<IActionResult> GetAutidLog()
    {
        var logs = await _BankService.GetBankAuditLog();
        var res = AuditMapper.MapToLogListDTO(logs);
        return Ok(res);
    }


}