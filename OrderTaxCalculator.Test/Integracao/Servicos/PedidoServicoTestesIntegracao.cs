using Microsoft.FeatureManagement;
using OrderTaxCalculator.Data.Repositorios;
using OrderTaxCalculator.Domain.Constantes;
using OrderTaxCalculator.Domain.Entidades;
using OrderTaxCalculator.Domain.Enumeradores;
using OrderTaxCalculator.Domain.Servicos;

namespace OrderTaxCalculator.Test.Integracao.Servicos;

public class PedidoServicoTestesIntegracao : BaseTestesIntegracao
{
    private readonly RepositorioPedido _pedidoRepositorio;
    private readonly IFeatureManager _featureManager;
    private readonly PedidoServico _pedidoServico;

    public PedidoServicoTestesIntegracao(BancoDeDadosFixture fixture) : base(fixture)
    {
        _pedidoRepositorio = new RepositorioPedido(DbContext);
        _featureManager = Substitute.For<IFeatureManager>();
        _pedidoServico = new PedidoServico(_pedidoRepositorio, _featureManager);
    }

    [Fact]
    public async Task CriePedidoAsync_DevePersistirPedidoComImposto_QuandoFeatureFlagDesativada()
    {
        // Arrange
        var pedidoId = 2001;
        var clienteId = 3001;
        var pedido = new Pedido(pedidoId, clienteId);
        
        var item = new PedidoItens(pedidoId, 4001, 2, 100m);
        pedido.AdicioneItem(item);

        _featureManager.IsEnabledAsync(ConstantesDomain.ImpostoReformaTributariaFeatureFlag)
            .Returns(false);

        // Act
        var resultado = await _pedidoServico.CriePedidoAsync(pedido);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Status.Should().Be(StatusEnum.Criado);
        resultado.Imposto.Should().Be(60m); // 200 * 0.3 = 60 (taxa de 30%)
        
        // Verificar que foi persistido no banco
        var pedidoPersistido = await _pedidoRepositorio.ObtenhaPorIdAsync(resultado.PedidoId);
        pedidoPersistido.Should().NotBeNull();
        pedidoPersistido!.PedidoId.Should().Be(pedidoId);
        pedidoPersistido.ClienteId.Should().Be(clienteId);
        pedidoPersistido.Status.Should().Be(StatusEnum.Criado);
        pedidoPersistido.Imposto.Should().Be(60m);
    }

    [Fact]
    public async Task CriePedidoAsync_DevePersistirPedidoComImposto_QuandoFeatureFlagAtivada()
    {
        // Arrange
        var pedidoId = 2002;
        var clienteId = 3002;
        var pedido = new Pedido(pedidoId, clienteId);
        
        var item = new PedidoItens(pedidoId, 4002, 2, 100m);
        pedido.AdicioneItem(item);

        _featureManager.IsEnabledAsync(ConstantesDomain.ImpostoReformaTributariaFeatureFlag)
            .Returns(true);

        // Act
        var resultado = await _pedidoServico.CriePedidoAsync(pedido);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Status.Should().Be(StatusEnum.Criado);
        resultado.Imposto.Should().Be(40m); // 200 * 0.2 = 40 (taxa de 20%)
        
        // Verificar que foi persistido no banco
        var pedidoPersistido = await _pedidoRepositorio.ObtenhaPorIdAsync(resultado.PedidoId);
        pedidoPersistido.Should().NotBeNull();
        pedidoPersistido.PedidoId.Should().Be(pedidoId);
        pedidoPersistido.ClienteId.Should().Be(clienteId);
        pedidoPersistido.Status.Should().Be(StatusEnum.Criado);
        pedidoPersistido.Imposto.Should().Be(40m);
    }

    [Fact]
    public async Task ObtenhaPedidoPorStatusAsync_DeveRetornarPedidosCorretos()
    {
        // Arrange
        var pedido1 = new Pedido(2003, 3003);
        pedido1.ApliqueStatus(StatusEnum.Criado);
        
        var pedido2 = new Pedido(2004, 3004);
        pedido2.ApliqueStatus(StatusEnum.Criado);
        
        var pedido3 = new Pedido(2005, 3005);
        pedido3.ApliqueStatus(StatusEnum.Criado);

        await DbContext.Pedidos.AddRangeAsync(pedido1, pedido2, pedido3);
        await DbContext.SaveChangesAsync();

        // Act
        var resultadoCriados = await _pedidoServico.ObtenhaPedidoPorStatusAsync("Criado");

        // Assert
        resultadoCriados.Should().HaveCount(3);
        resultadoCriados.Should().Contain(p => p.PedidoId == 2003);
        resultadoCriados.Should().Contain(p => p.PedidoId == 2004);
    }
}