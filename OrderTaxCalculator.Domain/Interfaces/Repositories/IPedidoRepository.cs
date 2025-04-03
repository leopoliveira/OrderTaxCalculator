using System.Linq.Expressions;
using OrderTaxCalculator.Domain.Entities;

namespace OrderTaxCalculator.Domain.Interfaces.Repositories;

public interface IPedidoRepository
{
    Task<Pedido?> GetByIdAsync(long id);
    Task<IReadOnlyList<Pedido>> GetByFilter(Expression<Func<Pedido, bool>> filter);
    Task<bool> PedidoExisteAsync(long pedidoId);
    Task AddAsync(Pedido pedido);
    Task UpdateAsync(Pedido pedido);
    void Delete(Pedido pedido);
    Task SaveChangesAsync();
}