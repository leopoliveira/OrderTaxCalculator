using Microsoft.EntityFrameworkCore;
using OrderTaxCalculator.Data.Repositorios;
using OrderTaxCalculator.Domain.Entidades;
using OrderTaxCalculator.Domain.Enumeradores;
using OrderTaxCalculator.Domain.Interfaces.Repositorios;

namespace OrderTaxCalculator.Test.Integracao.Repositorios;

public class PedidoRepositorioTestesIntegracao : BaseTestesIntegracao
{
    private readonly RepositorioPedido _pedidoRepositorio;

    public PedidoRepositorioTestesIntegracao(BancoDeDadosFixture fixture) : base(fixture)
    {
        _pedidoRepositorio = new RepositorioPedido(DbContext);
    }

    [Fact]
    public async Task ObtenhaPorIdAsync_DeveRetornarPedido_QuandoPedidoExiste()
    {
        // Arrange
        var pedido = new Pedido(1001, 2001);
        pedido.ApliqueStatus(StatusEnum.Criado);

        var item = new PedidoItens(1001, 3001, 2, 100m);
        pedido.AdicioneItem(item);

        await DbContext.Pedidos.AddAsync(pedido);
        await DbContext.SaveChangesAsync();

        // Act
        var resultado = await _pedidoRepositorio.ObtenhaPorIdAsync(1001);

        // Assert
        resultado.Should().NotBeNull();
        resultado.PedidoId.Should().Be(1001);
        resultado.ClienteId.Should().Be(2001);
        resultado.Status.Should().Be(StatusEnum.Criado);
        resultado.Itens.Should().HaveCount(1);
    }

    [Fact]
    public async Task ObtenhaPorFiltroAsync_DeveRetornarPedidos_QuandoStatusCriado()
    {
        // Arrange
        var pedido1 = new Pedido(1002, 2002);
        pedido1.ApliqueStatus(StatusEnum.Criado);

        var pedido2 = new Pedido(1003, 2003);
        pedido2.ApliqueStatus(StatusEnum.Criado);

        var pedido3 = new Pedido(1004, 2004);
        pedido3.ApliqueStatus(StatusEnum.Criado);

        await DbContext.Pedidos.AddRangeAsync(pedido1, pedido2, pedido3);
        await DbContext.SaveChangesAsync();

        // Act
        var resultado = await _pedidoRepositorio.ObtenhaPorFiltroAsync(p => p.Status == StatusEnum.Criado);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(3);
        resultado.Should().Contain(p => p.PedidoId == 1002);
        resultado.Should().Contain(p => p.PedidoId == 1003);
    }

    [Fact]
    public async Task InsiraAsync_DevePersistirPedido_ComItens()
    {
        // Arrange
        var pedido = new Pedido(1005, 2005);
        pedido.ApliqueStatus(StatusEnum.Criado);

        var item1 = new PedidoItens(1005, 3002, 1, 50m);
        var item2 = new PedidoItens(1005, 3003, 3, 75m);
        pedido.AdicioneItem(item1);
        pedido.AdicioneItem(item2);

        // Act
        await _pedidoRepositorio.InsiraAsync(pedido);
        await _pedidoRepositorio.SalveMudancasAsync();

        // Assert
        var pedidoSalvo = await DbContext.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.PedidoId == 1005);

        pedidoSalvo.Should().NotBeNull();
        pedidoSalvo!.ClienteId.Should().Be(2005);
        pedidoSalvo.Status.Should().Be(StatusEnum.Criado);
        pedidoSalvo.Itens.Should().HaveCount(2);
        pedidoSalvo.Itens.Should().Contain(i => i.ProdutoId == 3002 && i.Quantidade == 1 && i.Valor == 50m);
        pedidoSalvo.Itens.Should().Contain(i => i.ProdutoId == 3003 && i.Quantidade == 3 && i.Valor == 75m);
    }

    [Fact]
    public async Task PedidoExisteAsync_DeveRetornarTrue_QuandoPedidoExiste()
    {
        // Arrange
        var pedido = new Pedido(1006, 2006);
        await DbContext.Pedidos.AddAsync(pedido);
        await DbContext.SaveChangesAsync();

        // Act
        var resultado = await _pedidoRepositorio.PedidoExisteAsync(1006);

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task PedidoExisteAsync_DeveRetornarFalse_QuandoPedidoNaoExiste()
    {
        // Act
        var resultado = await _pedidoRepositorio.PedidoExisteAsync(9999);

        // Assert
        resultado.Should().BeFalse();
    }
}