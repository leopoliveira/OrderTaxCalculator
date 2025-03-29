namespace OrderTaxCalculator.Domain.Entities;

public class PedidoItens
{
    public long Id { get; set; }

    public long PedidoId { get; set; }

    public long ProdutoId { get; set; }

    public int Quantidade { get; set; }

    public decimal Valor { get; set; }
}