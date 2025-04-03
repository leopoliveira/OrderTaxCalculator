using System.Collections.ObjectModel;
using OrderTaxCalculator.API.Dto.Pedido;
using OrderTaxCalculator.Domain.Entities;

namespace OrderTaxCalculator.API.Mapeamentos;

public static class MapeamentoPedido
{
    public static Pedido ToPedido(this CriarPedidoRequest pedidoRequest)
    {
       var pedido = new Pedido(pedidoRequest.PedidoId, pedidoRequest.ClienteId);
       
       AddPedidoItens(pedidoRequest, pedido);

       return pedido;
    }
    
    public static CriarPedidoResponse ToCriarPedidoResponse(this Pedido pedido)
    {
        return new CriarPedidoResponse(pedido.PedidoId, pedido.Status.ToString());
    }

    public static ConsultarPedidoResponse ToConsultarPedidoResponse(this Pedido pedido)
    {
        var pedidoItens = GetPedidoItens(pedido);

        return new ConsultarPedidoResponse(
            pedido.Id,
            pedido.PedidoId,
            pedido.ClienteId,
            pedido.Imposto,
            pedidoItens,
            pedido.Status.ToString());
    }
    
    public static ReadOnlyCollection<ConsultarPedidoResponse> PedidosToListConsultarPedidoResponse(this IEnumerable<Pedido> pedidos)
    {
        return pedidos
            .Select(MapeamentoPedido.ToConsultarPedidoResponse)
            .ToList()
            .AsReadOnly();
    }
    
    private static void AddPedidoItens(CriarPedidoRequest request, Pedido pedido)
    {
        foreach (var item in request.Itens)
        {
            var pedidoItem = new PedidoItens(request.PedidoId, item.ProdutoId, item.Quantidade, item.Valor);
            pedido.AddItem(pedidoItem);
        }
    }

    private static ReadOnlyCollection<ItemPedidoResponse> GetPedidoItens(Pedido pedido)
    {
        return pedido.Itens
            .Select(item => new ItemPedidoResponse(item.ProdutoId, item.Quantidade, item.Valor))
            .ToList()
            .AsReadOnly();
    }
}