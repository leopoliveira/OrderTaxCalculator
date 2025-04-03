namespace OrderTaxCalculator.API.Dto.Pedido;

public record ItemPedidoRequest(long ProdutoId, int Quantidade , decimal Valor);