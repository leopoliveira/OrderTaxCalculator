namespace OrderTaxCalculator.API.Dto.Pedido;

public record ItemPedidoResponse(long ProdutoId, int Quantidade, decimal Valor);