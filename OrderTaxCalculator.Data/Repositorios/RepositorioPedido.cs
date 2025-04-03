using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OrderTaxCalculator.Data.BancoDeDados;
using OrderTaxCalculator.Domain.Entidades;
using OrderTaxCalculator.Domain.Interfaces.Repositorios;

namespace OrderTaxCalculator.Data.Repositorios;

public class RepositorioPedido : IPedidoRepositorio
{
    private readonly PedidoDbContext _context;

    public RepositorioPedido(PedidoDbContext context)
    {
        _context = context;
    }
    
    public async Task<Pedido?> ObtenhaPorIdAsync(long pedidoId)
    {
        return await _context.Pedidos
            .AsNoTracking()
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.PedidoId == pedidoId);
    }

    public async Task<IReadOnlyList<Pedido>> ObtenhaPorFiltroAsync(Expression<Func<Pedido, bool>> filter)
    {
        return await _context.Pedidos
            .AsNoTracking()
            .Include(p => p.Itens)
            .Where(filter)
            .ToListAsync();
    }

    public async Task<bool> PedidoExisteAsync(long pedidoId)
    {
        return await _context.Pedidos.AnyAsync(p => p.PedidoId == pedidoId);
    }

    public async Task InsiraAsync(Pedido pedido)
    {
        await _context.Pedidos.AddAsync(pedido);
    }

    public async Task SalveMudancasAsync()
    {
        await _context.SaveChangesAsync();
    }
}