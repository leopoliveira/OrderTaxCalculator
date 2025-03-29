using OrderTaxCalculator.Domain.Enumerator;

namespace OrderTaxCalculator.Domain.Entities;

public class Pedido
{
    public long Id { get; set; }

    public long PedidoId { get; set; }

    public long ClienteId { get; set; }

    public decimal Imposto { get; set; }

    public Status Status { get; set; }
}