using Microsoft.AspNetCore.Mvc;
using OrderTaxCalculator.API.Autenticacao.Interfaces;
using OrderTaxCalculator.API.Constantes;
using OrderTaxCalculator.API.Dto.Jwt;

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
    [ProducesResponseType(typeof(JwtResponse), StatusCodes.Status200OK)]
    public IActionResult GereDevToken([FromQuery] string clientId = "dev-client")
    {
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        var token = _jwtService.GereToken(clientId);
        var expiresAt = DateTime.UtcNow.AddMinutes(60);
        
        return Ok(new JwtResponse(token, expiresAt, "Bearer"));
    }
}