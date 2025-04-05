using Microsoft.AspNetCore.Mvc;
using OrderTaxCalculator.API.Autenticacao.Interfaces;
using OrderTaxCalculator.API.Constantes;

namespace OrderTaxCalculator.API.Controllers.v1;

[ApiController]
[Route(RotasApi.DevToken.Rota)]
public class DevTokenController : ControllerBase
{
    private readonly IJwtServico _jwtService;
    private readonly IWebHostEnvironment _environment;

    public DevTokenController(IJwtServico jwtService, IWebHostEnvironment environment)
    {
        _jwtService = jwtService;
        _environment = environment;
    }

    [HttpGet("generate")]
    public IActionResult GenerateToken([FromQuery] string clientId = "dev-client")
    {
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        var token = _jwtService.GereToken(clientId);
        
        return Ok(new
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            Type = "Bearer"
        });
    }
}