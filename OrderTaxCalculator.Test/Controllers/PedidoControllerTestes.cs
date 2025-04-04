using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrderTaxCalculator.API.Constantes;
using OrderTaxCalculator.API.Controllers.v1;
using OrderTaxCalculator.API.Dto.Pedido;
using OrderTaxCalculator.Domain.Entidades;
using OrderTaxCalculator.Domain.Enumeradores;
using OrderTaxCalculator.Domain.Interfaces.Servicos;

namespace OrderTaxCalculator.Test.Controllers;

public class PedidoControllerTestes : BaseTestes
{
    private readonly IPedidoServico _pedidoServico;
    private readonly ILogger<PedidoController> _logger;
    private readonly PedidoController _controller;
    private readonly Faker _faker;

    public PedidoControllerTestes()
    {
        _pedidoServico = Substitute.For<IPedidoServico>();
        _logger = Substitute.For<ILogger<PedidoController>>();
        _controller = new PedidoController(_pedidoServico, _logger);
        _faker = new Faker("pt_BR");
    }

    [Fact]
    public async Task CriarPedido_DeveRetornarCreatedAtActionResult_QuandoPedidoForCriado()
    {
        // Arrange
        var pedidoId = _faker.Random.Long(1, 1000);
        var clienteId = _faker.Random.Long(1, 100);
        var criarPedidoRequest = CrieCriarPedidoRequest(pedidoId, clienteId, _faker);
        var fakePedido = new Pedido(pedidoId, clienteId);
        fakePedido.ApliqueStatus(StatusEnum.Criado);
        
        _pedidoServico.CriePedidoAsync(Arg.Any<Pedido>())!
            .Returns(Task.FromResult(fakePedido));

        // Act
       var resultado = await _controller.CriarPedido(criarPedidoRequest);

        // Assert
        var actionResult = resultado.Result as CreatedAtActionResult;
        actionResult.Should().NotBeNull();
        actionResult.ActionName.Should().Be(nameof(PedidoController.GetPedidoPorId));
        actionResult.RouteValues!["pedidoId"].Should().Be(pedidoId);
        actionResult.StatusCode.Should().Be(StatusCodes.Status201Created);
            
        var responseValue = actionResult.Value as CriarPedidoResponse;
        responseValue.Should().NotBeNull();
        responseValue!.PedidoId.Should().Be(pedidoId);
        responseValue.Status.Should().Be(StatusEnum.Criado.ToString());
            
        await _pedidoServico.Received(1).CriePedidoAsync(Arg.Any<Pedido>());
    }

    [Fact]
    public async Task CriarPedido_DeveRetornarBadRequest_QuandoPedidoForDuplicado()
    {
        // Arrange
        var pedidoId = _faker.Random.Long(1, 1000);
        var clienteId = _faker.Random.Long(1, 100);
        var criarPedidoRequest = CrieCriarPedidoRequest(pedidoId, clienteId, _faker);
        
        _pedidoServico.CriePedidoAsync(Arg.Any<Pedido>())
            .Returns(Task.FromResult<Pedido?>(null));

        // Act
        var result = await _controller.CriarPedido(criarPedidoRequest);

        // Assert
        var actionResult = result.Result as BadRequestObjectResult;
        actionResult.Should().NotBeNull();
        actionResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            
        var problema = actionResult.Value as ProblemDetails;
        problema.Should().NotBeNull();
        problema!.Title.Should().Be(ConstantesApi.PedidoDuplicado);
        problema.Detail.Should().Contain(pedidoId.ToString());
            
        await _pedidoServico.Received(1).CriePedidoAsync(Arg.Any<Pedido>());
    }

    [Fact]
    public async Task GetPedidoPorId_DeveRetornarOkResult_QuandoPedidoForEncontrado()
    {
        // Arrange
        var pedidoId = _faker.Random.Long(1, 1000);
        var clienteId = _faker.Random.Long(1, 100);
        var fakePedido = new Pedido(pedidoId, clienteId);
        fakePedido.ApliqueStatus(StatusEnum.Criado);
        
        var item = new PedidoItens(
            pedidoId,
            _faker.Random.Long(1, 50),
            _faker.Random.Int(1, 10),
            _faker.Random.Decimal(10, 100)
        );
        fakePedido.AdicioneItem(item);
        
        _pedidoServico.ObtenhaPedidoPorIdAsync(pedidoId)!
            .Returns(Task.FromResult(fakePedido));

        // Act
        var resultado = await _controller.GetPedidoPorId(pedidoId);

        // Assert
        var actionResult = resultado.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        actionResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            
        var resposta = actionResult.Value as ConsultarPedidoResponse;
        resposta.Should().NotBeNull();
        resposta.PedidoId.Should().Be(pedidoId);
        resposta.ClienteId.Should().Be(clienteId);
        resposta.Status.Should().Be(StatusEnum.Criado.ToString());
        resposta.Itens.Should().HaveCount(1);
            
        await _pedidoServico.Received(1).ObtenhaPedidoPorIdAsync(pedidoId);
    }

    [Fact]
    public async Task GetPedidoPorId_DeveRetornarNotFound_QuandoPedidoNaoForEncontrado()
    {
        // Arrange
        var pedidoId = _faker.Random.Long(1, 1000);
        _pedidoServico.ObtenhaPedidoPorIdAsync(pedidoId)
            .Returns(Task.FromResult<Pedido?>(null));

        // Act
        var resultado = await _controller.GetPedidoPorId(pedidoId);

        // Assert
        var actionResult = resultado.Result as NotFoundObjectResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            
        var problem = actionResult.Value as ProblemDetails;
        problem.Should().NotBeNull();
        problem.Title.Should().Be(ConstantesApi.PedidoNaoEncontrado);
        problem.Detail.Should().Contain(pedidoId.ToString());
            
        await _pedidoServico.Received(1).ObtenhaPedidoPorIdAsync(pedidoId);
    }
    
    [Fact]
    public async Task ListarPedidos_DeveRetornarOk_ComListaDePedidos()
    {
        // Arrange
        var status = StatusEnum.Criado.ToString();
        
        var pedidos = new List<Pedido>
        {
            new (_faker.Random.Long(1, 1000), _faker.Random.Long(1, 100)),
            new (_faker.Random.Long(1, 1000), _faker.Random.Long(1, 100))
        };
        
        pedidos[0].ApliqueStatus(StatusEnum.Criado);
        pedidos[1].ApliqueStatus(StatusEnum.Criado);
        
        var item1 = CriePedidoItens(pedidos[0]);
        pedidos[0].AdicioneItem(item1);
        
        var item2 = CriePedidoItens(pedidos[1]);
        pedidos[1].AdicioneItem(item2);
        
        _pedidoServico.ObtenhaPedidoPorStatusAsync(status).Returns(pedidos);
        
        // Act
        var resultado = await _controller.ListarPedidos(status);
        
        // Assert
        var actionResult = resultado.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        actionResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        
        var responseValue = actionResult.Value as ReadOnlyCollection<ConsultarPedidoResponse>;
        responseValue.Should().NotBeNull();
        responseValue.Should().HaveCount(2);
        
        await _pedidoServico.Received(1).ObtenhaPedidoPorStatusAsync(status);
    }

    [Fact]
    public async Task ListarPedidos_DeveRetornarOk_ComListaVazia_QuandoNenhumPedidoEncontrado()
    {
        // Arrange
        var status = StatusEnum.Criado.ToString();
        
        _pedidoServico.ObtenhaPedidoPorStatusAsync(status).Returns(new List<Pedido>());
        
        // Act
        var resultado = await _controller.ListarPedidos(status);
        
        // Assert
        var actionResult = resultado.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        
        var responseValue = actionResult.Value as ReadOnlyCollection<ConsultarPedidoResponse>;
        responseValue.Should().NotBeNull();
        responseValue.Should().BeEmpty();
        
        await _pedidoServico.Received(1).ObtenhaPedidoPorStatusAsync(status);
    }
}