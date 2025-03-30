using OrderTaxCalculator.Domain.Enumerator;

namespace OrderTaxCalculator.Domain.Entities;

public class Pedido
{
    private List<PedidoItens> _pedidoItens = [];
    
    public Pedido() { }
    public Pedido(long pedidoId, long clienteId, decimal imposto, Status status)
    {
        PedidoId = pedidoId;
        ClienteId = clienteId;
        Imposto = imposto;
        Status = status;
    }
    
    public long Id { get; private set; }

    public long PedidoId { get; private set; }

    public long ClienteId { get; private set; }

    public decimal Imposto { get; private set; }
    
    public Status Status { get; private set; }

    public IReadOnlyList<PedidoItens> Itens => _pedidoItens.AsReadOnly();

    public void AddItem(PedidoItens pedidoItens) => _pedidoItens.Add(pedidoItens);

    public void RemoveItem(PedidoItens pedidoItens) => _pedidoItens.Remove(pedidoItens);
}