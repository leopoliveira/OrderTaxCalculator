using Microsoft.EntityFrameworkCore;
using OrderTaxCalculator.Domain.Entities;

namespace OrderTaxCalculator.Data.Database;

public class PedidoDbContext : DbContext
{
    public PedidoDbContext(DbContextOptions<PedidoDbContext> options) : base(options) { }
    
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<PedidoItens> PedidoItens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var assembly = GetType().Assembly;
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);
    }
}