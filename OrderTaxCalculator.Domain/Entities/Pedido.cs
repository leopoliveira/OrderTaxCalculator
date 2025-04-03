using OrderTaxCalculator.Domain.Enumerator;

namespace OrderTaxCalculator.Domain.Entities;

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

    public void AddItem(PedidoItens pedidoItens) => _pedidoItens.Add(pedidoItens);

    public decimal ValorTotalItens => _pedidoItens.Sum(pi => pi.Valor);
    
    public void SetStatus(StatusEnum novoStatus) => Status = novoStatus;
    
    public void SetImposto(decimal imposto) => Imposto = imposto;
}