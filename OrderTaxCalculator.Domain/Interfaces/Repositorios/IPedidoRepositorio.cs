using System.Linq.Expressions;
using OrderTaxCalculator.Domain.Entidades;

namespace OrderTaxCalculator.Domain.Interfaces.Repositorios;

public interface IPedidoRepositorio
{
    Task<Pedido?> ObtenhaPorIdAsync(long pedidoId);
    Task<IReadOnlyList<Pedido>> ObtenhaPorFiltroAsync(Expression<Func<Pedido, bool>> filter);
    Task<bool> PedidoExisteAsync(long pedidoId);
    Task InsiraAsync(Pedido pedido);
    Task SalveMudancasAsync();
}