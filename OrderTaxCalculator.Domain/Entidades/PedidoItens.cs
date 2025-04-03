namespace OrderTaxCalculator.Domain.Entidades;

public class PedidoItens
{
    private PedidoItens() { }
    public PedidoItens(long pedidoId, long produtoId, int quantidade, decimal valor)
    {
        PedidoId = pedidoId;
        ProdutoId = produtoId;
        Quantidade = quantidade;
        Valor = valor;
    }
    
    public long Id { get; private set; }

    public long PedidoId { get; private set; }

    public long ProdutoId { get; private set; }

    public int Quantidade { get; private set; }

    public decimal Valor { get; private set; }
}