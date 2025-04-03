using Microsoft.EntityFrameworkCore;
using OrderTaxCalculator.Domain.Entidades;

namespace OrderTaxCalculator.Data.BancoDeDados;

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