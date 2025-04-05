using OrderTaxCalculator.API.Mapeamentos;
using OrderTaxCalculator.Domain.Entidades;
using OrderTaxCalculator.Domain.Enumeradores;

namespace OrderTaxCalculator.Test.Mapeadores;

public class MapeamentoPedidoTestes : BaseTestes
{
    private readonly Faker _faker;
        
        public MapeamentoPedidoTestes()
        {
            _faker = new Faker("pt_BR");
        }
        
        [Fact]
        public void ParaPedido_DeveMapearCorretamente_CriarPedidoRequestParaPedido()
        {
            // Arrange
            var pedidoId = _faker.Random.Long(1, 1000);
            var clienteId = _faker.Random.Long(1, 100);
            var criarPedidoRequest = CrieCriarPedidoRequest(pedidoId, clienteId, _faker);
            
            // Act
            var pedido = criarPedidoRequest.ParaPedido();
            
            // Assert
            pedido.Should().NotBeNull();
            pedido.PedidoId.Should().Be(criarPedidoRequest.PedidoId);
            pedido.ClienteId.Should().Be(criarPedidoRequest.ClienteId);
            pedido.Itens.Should().HaveCount(criarPedidoRequest.Itens.Count);
            
            for (var i = 0; i < criarPedidoRequest.Itens.Count; i++)
            {
                var requestItem = criarPedidoRequest.Itens[i];
                var pedidoItem = pedido.Itens[i];
                
                pedidoItem.PedidoId.Should().Be(pedidoId);
                pedidoItem.ProdutoId.Should().Be(requestItem.ProdutoId);
                pedidoItem.Quantidade.Should().Be(requestItem.Quantidade);
                pedidoItem.Valor.Should().Be(requestItem.Valor);
            }
        }
        
        [Fact]
        public void ParaCriarPedidoResponse_DeveMapearCorretamente_PedidoParaCriarPedidoResponse()
        {
            // Arrange
            var pedidoId = _faker.Random.Long(1, 1000);
            var clienteId = _faker.Random.Long(1, 100);
            
            var pedido = new Pedido(pedidoId, clienteId);
            pedido.ApliqueStatus(StatusEnum.Criado);
            
            // Act
            var response = pedido.ParaCriarPedidoResponse();
            
            // Assert
            response.Should().NotBeNull();
            response.PedidoId.Should().Be(pedido.PedidoId);
            response.Status.Should().Be(pedido.Status.ToString());
        }
        
        [Fact]
        public void ParaConsultarPedidoResponse_DeveMapearCorretamente_PedidoParaConsultarPedidoResponse()
        {
            // Arrange
            var pedidoId = _faker.Random.Long(1, 1000);
            var clienteId = _faker.Random.Long(1, 100);
            
            var pedido = new Pedido(pedidoId, clienteId);
            pedido.ApliqueStatus(StatusEnum.Criado);
            pedido.ApliqueImposto(_faker.Random.Decimal(1, 50));

            var item1 = CriePedidoItens(pedido);
            pedido.AdicioneItem(item1);
            
            var item2 = CriePedidoItens(pedido);
            pedido.AdicioneItem(item2);
            
            // Act
            var response = pedido.ParaConsultarPedidoResponse();
            
            // Assert
            response.Should().NotBeNull();
            response.Id.Should().Be(pedido.Id);
            response.PedidoId.Should().Be(pedido.PedidoId);
            response.ClienteId.Should().Be(pedido.ClienteId);
            response.Imposto.Should().Be(pedido.Imposto);
            response.Status.Should().Be(pedido.Status.ToString());
            response.Itens.Should().HaveCount(2);
            
            // Verificar mapeamento de cada item
            response.Itens[0].ProdutoId.Should().Be(item1.ProdutoId);
            response.Itens[0].Quantidade.Should().Be(item1.Quantidade);
            response.Itens[0].Valor.Should().Be(item1.Valor);
            
            response.Itens[1].ProdutoId.Should().Be(item2.ProdutoId);
            response.Itens[1].Quantidade.Should().Be(item2.Quantidade);
            response.Itens[1].Valor.Should().Be(item2.Valor);
        }
        
        [Fact]
        public void ParaListConsultarPedidoResponse_DeveMapearCorretamente_ListaPedidosParaListaConsultarPedidoResponse()
        {
            // Arrange
            var pedidos = new List<Pedido>
            {
                new Pedido(_faker.Random.Long(1, 1000), _faker.Random.Long(1, 100)),
                new Pedido(_faker.Random.Long(1, 1000), _faker.Random.Long(1, 100))
            };
            
            pedidos[0].ApliqueStatus(StatusEnum.Criado);
            pedidos[1].ApliqueStatus(StatusEnum.Criado);
            
            var item1 = CriePedidoItens(pedidos[0]);
            pedidos[0].AdicioneItem(item1);
            
            var item2 = CriePedidoItens(pedidos[1]);
            pedidos[1].AdicioneItem(item2);
            
            // Act
            var responses = pedidos.ParaListConsultarPedidoResponse();
            
            // Assert
            responses.Should().NotBeNull();
            responses.Should().HaveCount(2);
            
            responses[0].PedidoId.Should().Be(pedidos[0].PedidoId);
            responses[0].ClienteId.Should().Be(pedidos[0].ClienteId);
            responses[0].Status.Should().Be(StatusEnum.Criado.ToString());
            responses[0].Itens.Should().HaveCount(1);
            
            responses[1].PedidoId.Should().Be(pedidos[1].PedidoId);
            responses[1].ClienteId.Should().Be(pedidos[1].ClienteId);
            responses[1].Status.Should().Be(StatusEnum.Criado.ToString());
            responses[1].Itens.Should().HaveCount(1);
        }
}