using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderTaxCalculator.Domain.Entities;

namespace OrderTaxCalculator.Data.Database.EntitiesConfiguration;

public class PedidoItensConfiguracao : IEntityTypeConfiguration<PedidoItens>
{
    public void Configure(EntityTypeBuilder<PedidoItens> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
    }
}