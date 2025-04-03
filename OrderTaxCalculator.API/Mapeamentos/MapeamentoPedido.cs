using System.Collections.ObjectModel;
using OrderTaxCalculator.API.Dto.Pedido;
using OrderTaxCalculator.Domain.Entidades;

namespace OrderTaxCalculator.API.Mapeamentos;

public static class MapeamentoPedido
{
    public static Pedido ParaPedido(this CriarPedidoRequest pedidoRequest)
    {
       var pedido = new Pedido(pedidoRequest.PedidoId, pedidoRequest.ClienteId);
       
       AdicionePedidoItens(pedidoRequest, pedido);

       return pedido;
    }
    
    public static CriarPedidoResponse ParaCriarPedidoResponse(this Pedido pedido)
    {
        return new CriarPedidoResponse(pedido.PedidoId, pedido.Status.ToString());
    }

    public static ConsultarPedidoResponse ParaConsultarPedidoResponse(this Pedido pedido)
    {
        var pedidoItens = ObtenhaPedidoItens(pedido);

        return new ConsultarPedidoResponse(
            pedido.Id,
            pedido.PedidoId,
            pedido.ClienteId,
            pedido.Imposto,
            pedidoItens,
            pedido.Status.ToString());
    }
    
    public static ReadOnlyCollection<ConsultarPedidoResponse> ParaListConsultarPedidoResponse(this IEnumerable<Pedido> pedidos)
    {
        return pedidos
            .Select(MapeamentoPedido.ParaConsultarPedidoResponse)
            .ToList()
            .AsReadOnly();
    }
    
    private static void AdicionePedidoItens(CriarPedidoRequest request, Pedido pedido)
    {
        foreach (var item in request.Itens)
        {
            var pedidoItem = new PedidoItens(request.PedidoId, item.ProdutoId, item.Quantidade, item.Valor);
            pedido.AdicioneItem(pedidoItem);
        }
    }

    private static ReadOnlyCollection<ItemPedidoResponse> ObtenhaPedidoItens(Pedido pedido)
    {
        return pedido.Itens
            .Select(item => new ItemPedidoResponse(item.ProdutoId, item.Quantidade, item.Valor))
            .ToList()
            .AsReadOnly();
    }
}