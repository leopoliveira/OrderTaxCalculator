using OrderTaxCalculator.Domain.Entities;
using OrderTaxCalculator.Domain.Enumerator;

namespace OrderTaxCalculator.Domain.Interfaces.Services;

public interface IPedidoService
{
    Task<Pedido?> GetPedidoByIdAsync(long pedidoId);
    Task<IReadOnlyList<Pedido>> GetPedidoByStatus(string status);
    Task<bool> PedidoExiste(long id);
    Task<Pedido?> CreatePedidoAsync(Pedido pedido);
    Task UpdatePedidoAsync(Pedido pedido);
    Task DeletePedidoAsync(Pedido pedido);
}