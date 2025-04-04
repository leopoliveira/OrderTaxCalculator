using System.Linq.Expressions;
using Microsoft.FeatureManagement;
using OrderTaxCalculator.Domain.Constantes;
using OrderTaxCalculator.Domain.Entidades;
using OrderTaxCalculator.Domain.Enumeradores;
using OrderTaxCalculator.Domain.Interfaces.Repositorios;
using OrderTaxCalculator.Domain.Servicos;

namespace OrderTaxCalculator.Test.Servicos;

public class PedidoServicoTestes
{
    private readonly IPedidoRepositorio _pedidoRepositorio;
    private readonly IFeatureManager _featureManager;
    private readonly PedidoServico _pedidoServico;
    private readonly Faker _faker;

    public PedidoServicoTestes()
    {
        _pedidoRepositorio = Substitute.For<IPedidoRepositorio>();
        _featureManager = Substitute.For<IFeatureManager>();
        _pedidoServico = new PedidoServico(_pedidoRepositorio, _featureManager);
        _faker = new Faker("pt_BR");
    }

    [Fact]
    public async Task ObtenhaPedidoPorIdAsync_DeveRetornarPedido_QuandoPedidoExiste()
    {
        // Arrange
        var pedidoId = _faker.Random.Long(1, 1000);
        var clienteId = new Faker().Random.Long(1, 100);
        var fakePedido = new Pedido(pedidoId, clienteId);

        _pedidoRepositorio.ObtenhaPorIdAsync(pedidoId)!
            .Returns(Task.FromResult(fakePedido));

        // Act
        var resultado = await _pedidoServico.ObtenhaPedidoPorIdAsync(pedidoId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEquivalentTo(fakePedido);
        await _pedidoRepositorio.Received(1).ObtenhaPorIdAsync(pedidoId);
    }

    [Fact]
    public async Task ObtenhaPedidoPorIdAsync_DeveRetornarNull_QuandoPedidoNaoExiste()
    {
        // Arrange
        var pedidoId = _faker.Random.Long(1, 1000);
        _pedidoRepositorio.ObtenhaPorIdAsync(pedidoId).Returns(Task.FromResult<Pedido?>(null));

        // Act
        var resultado = await _pedidoServico.ObtenhaPedidoPorIdAsync(pedidoId);

        // Assert
        resultado.Should().BeNull();
        await _pedidoRepositorio.Received(1).ObtenhaPorIdAsync(pedidoId);
    }

    [Fact]
    public async Task ObtenhaPedidoPorStatusAsync_DeveRetornarListaVazia_QuandoStatusInvalido()
    {
        // Arrange
        var statusInvalido = "StatusInexistente";

        // Act
        var resultado = await _pedidoServico.ObtenhaPedidoPorStatusAsync(statusInvalido);

        // Assert
        resultado.Should().BeEmpty();
        await _pedidoRepositorio.DidNotReceive().ObtenhaPorFiltroAsync(Arg.Any<Expression<Func<Pedido, bool>>>());
    }

    [Fact]
    public async Task ObtenhaPedidoPorStatusAsync_DeveRetornarListaDePedidos_QuandoStatusValido()
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

        _pedidoRepositorio
            .ObtenhaPorFiltroAsync(Arg.Any<Expression<Func<Pedido, bool>>>())
            .Returns(pedidos);

        // Act
        var resultado = await _pedidoServico.ObtenhaPedidoPorStatusAsync(status);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Count.Should().Be(pedidos.Count);
        await _pedidoRepositorio.Received(1).ObtenhaPorFiltroAsync(Arg.Any<Expression<Func<Pedido, bool>>>());
    }
    
    [Fact]
    public async Task PedidoExisteAsync_DeveRetornarTrue_QuandoPedidooExiste()
    {
        // Arrange
        var pedidoId = _faker.Random.Long(1, 1000);
        _pedidoRepositorio.PedidoExisteAsync(pedidoId)
            .Returns(Task.FromResult(true));

        // Act
        var resultado = await _pedidoServico.PedidoExisteAsync(pedidoId);

        // Assert
        resultado.Should().BeTrue();
        await _pedidoRepositorio.Received(1).PedidoExisteAsync(pedidoId);
    }
    
    [Fact]
    public async Task PedidoExisteAsync_DeveRetornarFalse_QuandoPedidoNaoExiste()
    {
        // Arrange
        var pedidoId = _faker.Random.Long(1, 1000);
        _pedidoRepositorio.PedidoExisteAsync(pedidoId).Returns(false);

        // Act
        var resultado = await _pedidoServico.PedidoExisteAsync(pedidoId);

        // Assert
        resultado.Should().BeFalse();
        await _pedidoRepositorio.Received(1).PedidoExisteAsync(pedidoId);
    }

    [Fact]
    public async Task CriePedidoAsync_DeveRetornarNull_QuandoPedidoJaoExiste()
    {
        // Arrange
        var pedidoId = _faker.Random.Long(1, 1000);
        var clienteId = _faker.Random.Long(1, 100);
        var fakePedido = new Pedido(pedidoId, clienteId);

        _pedidoRepositorio.PedidoExisteAsync(pedidoId)
            .Returns(Task.FromResult(true));

        // Act
        var resultado = await _pedidoServico.CriePedidoAsync(fakePedido);

        // Assert
        resultado.Should().BeNull();
        await _pedidoRepositorio.Received(1).PedidoExisteAsync(pedidoId);
        await _pedidoRepositorio.DidNotReceive().InsiraAsync(Arg.Any<Pedido>());
        await _pedidoRepositorio.DidNotReceive().SalveMudancasAsync();
    }

    [Fact]
    public async Task CriePedidoAsync_DeveCriarPedido_QuandoPedidoNaoExiste_ComFeatureFlagDesativada()
    {
        // Arrange
        var pedidoId = _faker.Random.Long(1, 1000);
        var clienteId = _faker.Random.Long(1, 100);
        var pedido = new Pedido(pedidoId, clienteId);
        var item = new PedidoItens(
            pedidoId,
            _faker.Random.Long(1, 50),
            _faker.Random.Int(1, 10),
            _faker.Random.Decimal(10, 100)
        );
        pedido.AdicioneItem(item);
        
        _pedidoRepositorio.PedidoExisteAsync(pedidoId).Returns(false);
        _featureManager.IsEnabledAsync(ConstantesDomain.ImpostoReformaTributariaFeatureFlag).Returns(false);

        // Act
        var resultado = await _pedidoServico.CriePedidoAsync(pedido);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Status.Should().Be(StatusEnum.Criado);
        resultado.Imposto.Should().BeGreaterThan(0);
        
        await _pedidoRepositorio.Received(1).PedidoExisteAsync(pedidoId);
        await _pedidoRepositorio.Received(1).InsiraAsync(pedido);
        await _pedidoRepositorio.Received(1).SalveMudancasAsync();
        await _featureManager.Received(1).IsEnabledAsync(ConstantesDomain.ImpostoReformaTributariaFeatureFlag);
    }

    [Fact]
    public async Task CriePedidoAsync_DeveCriarPedido_QuandoPedidoNaoExiste_ComFeatureFlagAtivada()
    {
        // Arrange
        var pedidoId = _faker.Random.Long(1, 1000);
        var clienteId = _faker.Random.Long(1, 100);
        var pedido = new Pedido(pedidoId, clienteId);
        
        var item = new PedidoItens(
            pedidoId,
            _faker.Random.Long(1, 50),
            _faker.Random.Int(1, 10),
            _faker.Random.Decimal(10, 100)
        );
        pedido.AdicioneItem(item);
        
        _pedidoRepositorio.PedidoExisteAsync(pedidoId).Returns(false);
        _featureManager.IsEnabledAsync(ConstantesDomain.ImpostoReformaTributariaFeatureFlag).Returns(true);

        // Act
        var resultado = await _pedidoServico.CriePedidoAsync(pedido);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Status.Should().Be(StatusEnum.Criado);
        resultado.Imposto.Should().BeGreaterThan(0);
        
        await _pedidoRepositorio.Received(1).PedidoExisteAsync(pedidoId);
        await _pedidoRepositorio.Received(1).InsiraAsync(pedido);
        await _pedidoRepositorio.Received(1).SalveMudancasAsync();
        await _featureManager.Received(1).IsEnabledAsync(ConstantesDomain.ImpostoReformaTributariaFeatureFlag);
    }
}