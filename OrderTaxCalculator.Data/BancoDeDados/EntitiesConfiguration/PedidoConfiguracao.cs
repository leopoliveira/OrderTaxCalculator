using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderTaxCalculator.Domain.Entidades;

namespace OrderTaxCalculator.Data.BancoDeDados.EntitiesConfiguration;

public class PedidoConfiguracao : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        
        builder.Property(x => x.Status)
            .HasConversion<string>();
        
        builder.Navigation(p => p.Itens)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasField("_pedidoItens");
        
        builder.HasMany(p => p.Itens)
            .WithOne()
            .HasForeignKey(p => p.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}