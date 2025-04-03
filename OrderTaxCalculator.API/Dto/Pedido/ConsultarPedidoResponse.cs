namespace OrderTaxCalculator.API.Dto.Pedido;

public record ConsultarPedidoResponse(long Id, long PedidoId, long ClienteId, decimal Imposto, IReadOnlyList<ItemPedidoResponse> Itens, string Status);