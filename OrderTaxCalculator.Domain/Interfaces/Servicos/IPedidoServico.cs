using OrderTaxCalculator.Domain.Entidades;

namespace OrderTaxCalculator.Domain.Interfaces.Servicos;

public interface IPedidoServico
{
    Task<Pedido?> ObtenhaPedidoPorIdAsync(long pedidoId);
    Task<IReadOnlyList<Pedido>> ObtenhaPedidoPorStatusAsync(string status);
    Task<bool> PedidoExisteAsync(long id);
    Task<Pedido?> CriePedidoAsync(Pedido pedido);
}