namespace OrderTaxCalculator.API.Dto.Pedido;

public record CriarPedidoRequest(long PedidoId, long ClienteId, List<ItemPedidoRequest> Itens);