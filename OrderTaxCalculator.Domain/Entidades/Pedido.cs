using OrderTaxCalculator.Domain.Enumeradores;

namespace OrderTaxCalculator.Domain.Entidades;

public class Pedido
{
    private List<PedidoItens> _pedidoItens = [];
    
    private Pedido() { }
    public Pedido(long pedidoId, long clienteId)
    {
        PedidoId = pedidoId;
        ClienteId = clienteId;
    }
    
    public long Id { get; private set; }

    public long PedidoId { get; private set; }

    public long ClienteId { get; private set; }

    public decimal Imposto { get; private set; }
    
    public StatusEnum Status { get; private set; }

    public IReadOnlyList<PedidoItens> Itens => _pedidoItens.AsReadOnly();

    public void AdicioneItem(PedidoItens pedidoItens) => _pedidoItens.Add(pedidoItens);

    public decimal ValorTotalItens => _pedidoItens.Sum(pi => pi.Valor * pi.Quantidade);
    
    public void ApliqueStatus(StatusEnum novoStatus) => Status = novoStatus;
    
    public void ApliqueImposto(decimal imposto) => Imposto = imposto;
}