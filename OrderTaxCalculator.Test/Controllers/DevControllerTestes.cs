using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using OrderTaxCalculator.API.Autenticacao.Interfaces;
using OrderTaxCalculator.API.Controllers.v1;
using OrderTaxCalculator.API.Dto.Jwt;

namespace OrderTaxCalculator.Test.Controllers;

public class DevTokenControllerTestes : BaseTestes
{
    private readonly IJwtServico _jwtServico;
    private readonly IWebHostEnvironment _environment;
    private readonly DevTokenController _controller;

    public DevTokenControllerTestes()
    {
        _jwtServico = Substitute.For<IJwtServico>();
        _environment = Substitute.For<IWebHostEnvironment>();
        _controller = new DevTokenController(_jwtServico, _environment);
    }

    [Fact]
    public void GenerateToken_DeveRetornarOk_QuandoAmbienteForDesenvolvimento()
    {
        // Arrange
        var clienteId = "test-client";
        var token = "jwt-token-teste";
        
        _environment.EnvironmentName = "Development";
        _jwtServico.GereToken(clienteId).Returns(token);

        // Act
        var resultado = _controller.GereDevToken(clienteId);

        // Assert
        var actionResult = resultado as OkObjectResult;
        actionResult.Should().NotBeNull();
        actionResult.StatusCode.Should().Be(200);

        var responseValue = actionResult.Value.Should().BeAssignableTo<JwtResponse>().Subject;
        
        var tokenResponse = responseValue.Token;
        var expiresAt = responseValue.ExpiresAt;
        var type = responseValue.Type;
        
        tokenResponse.Should().Be(token);
        expiresAt.Should().BeAfter(DateTime.UtcNow);
        type.Should().Be("Bearer");
        
        _jwtServico.Received(1).GereToken(clienteId);
    }

    [Fact]
    public void GenerateToken_DeveUsarDefaultClientId_QuandoNaoInformado()
    {
        // Arrange
        var clienteId = "dev-client";
        var token = "jwt-token-padrao";
        
        _environment.EnvironmentName = "Development";
        _jwtServico.GereToken(clienteId).Returns(token);

        // Act
        var resultado = _controller.GereDevToken();

        // Assert
        var actionResult = resultado as OkObjectResult;
        actionResult.Should().NotBeNull();
        
        _jwtServico.Received(1).GereToken(clienteId);
    }

    [Fact]
    public void GenerateToken_DeveRetornarNotFound_QuandoAmbienteNaoForDesenvolvimento()
    {
        // Arrange
        _environment.EnvironmentName = "Production";

        // Act
        var resultado = _controller.GereDevToken();

        // Assert
        var actionResult = resultado as NotFoundResult;
        actionResult.Should().NotBeNull();
        actionResult.StatusCode.Should().Be(404);
        
        _jwtServico.DidNotReceive().GereToken(Arg.Any<string>());
    }
}