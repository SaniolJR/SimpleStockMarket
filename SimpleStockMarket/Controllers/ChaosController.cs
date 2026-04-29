using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controllers;

[ApiController]
public class ChaosController : ControllerBase
{
    private readonly IChaosService _ChaosService;

    public ChaosController(IChaosService ChaosService)
    {
        _ChaosService = ChaosService;
    }

    [HttpPost("chaos")]
    public IActionResult Kill()
    {
        _ = Task.Run(async () =>
        {
            await Task.Delay(100);
            _ChaosService.Terminate(1);
        });
        return Ok();
    }
}