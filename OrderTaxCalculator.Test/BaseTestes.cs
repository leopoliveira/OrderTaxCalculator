using OrderTaxCalculator.API.Dto.Pedido;
using OrderTaxCalculator.Domain.Entidades;

namespace OrderTaxCalculator.Test;

public class BaseTestes
{
    private readonly Faker _faker = new();
    
    protected static CriarPedidoRequest CrieCriarPedidoRequest(long pedidoId, long clienteId, Faker faker)
    {
        var criarPedidoRequest = new CriarPedidoRequest(
            pedidoId,
            clienteId,
            [
                new ItemPedidoRequest(
                    faker.Random.Long(1, 50),
                    faker.Random.Int(1, 10),
                    faker.Finance.Amount()
                ),
                new ItemPedidoRequest(
                    faker.Random.Long(1, 50),
                    faker.Random.Int(1, 10),
                    faker.Finance.Amount()
                )
            ]
        );
        return criarPedidoRequest;
    }
    
    protected PedidoItens CriePedidoItens(Pedido pedido)
    {
        return new PedidoItens(
            pedido.PedidoId,
            _faker.Random.Long(1, 50),
            _faker.Random.Int(1, 10),
            _faker.Random.Decimal(10, 100)
        );
    }
}