using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OrderTaxCalculator.Data.Database;
using OrderTaxCalculator.Domain.Entities;
using OrderTaxCalculator.Domain.Interfaces.Repositories;

namespace OrderTaxCalculator.Data.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly PedidoDbContext _context;

    public PedidoRepository(PedidoDbContext context)
    {
        _context = context;
    }
    
    public async Task<Pedido?> GetByIdAsync(long pedidoId)
    {
        return await _context.Pedidos
            .AsNoTracking()
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.PedidoId == pedidoId);
    }

    public async Task<IReadOnlyList<Pedido>> GetByFilter(Expression<Func<Pedido, bool>> filter)
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

    public async Task AddAsync(Pedido pedido)
    {
        await _context.Pedidos.AddAsync(pedido);
    }
    
    public async Task UpdateAsync(Pedido pedido)
    {
        _context.Pedidos.Update(pedido);
        await Task.CompletedTask;
    }

    public void Delete(Pedido pedido)
    {
        _context.Pedidos.Remove(pedido);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}