using System.Net;
using System.Net.Http.Json;
using FluentAssertions; // Add this import
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderTaxCalculator.API.Constantes;
using OrderTaxCalculator.API.Dto.Pedido;
using OrderTaxCalculator.Data.BancoDeDados;
using OrderTaxCalculator.Domain.Enumeradores;
using Testcontainers.MsSql;

namespace OrderTaxCalculator.Test.Integracao.Controllers;

public class PedidoControllerTestesIntegracao
{
    public class PedidoControllerTests : IAsyncLifetime
    {
        private readonly MsSqlContainer _sqlContainer;
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        public PedidoControllerTests()
        {
            _sqlContainer = new MsSqlBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithPassword("StrongPassword@123")
                .WithCleanUp(true)
                .Build();
        }
        
        public async Task InitializeAsync()
        {
            await _sqlContainer.StartAsync();

            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        // Remova todos os registros de DbContext
                        var descriptors = services.Where(
                            d => d.ServiceType == typeof(DbContextOptions<PedidoDbContext>) ||
                                 d.ServiceType == typeof(DbContextOptions<PedidoDbContext>)).ToList();

                        foreach (var descriptor in descriptors)
                        {
                            services.Remove(descriptor);
                        }
                        
                        var dbProviderCoreServices = services.Where(
                            d => d.ServiceType.Name.Contains("DbContextOptions")).ToList();
                        
                        foreach (var descriptor in dbProviderCoreServices)
                        {
                            services.Remove(descriptor);
                        }

                        // Adicione o container
                        services.AddDbContext<PedidoDbContext>(options =>
                        {
                            options.UseSqlServer(_sqlContainer.GetConnectionString());
                        });

                        // Garanta a criação da base
                        var sp = services.BuildServiceProvider();
                        using var scope = sp.CreateScope();
                        var scopedServices = scope.ServiceProvider;
                        var db = scopedServices.GetRequiredService<PedidoDbContext>();
                        db.Database.EnsureCreated();
                    });
                });

            _client = _factory.CreateClient();
        }

        public async Task DisposeAsync()
        {
            _client.Dispose();
            _factory.Dispose();
            await _sqlContainer.DisposeAsync().AsTask();
        }
        
        [Fact]
        public async Task CriarPedido_DeveRetornarCreated_QuandoPedidoValido()
        {
            // Arrange
            var request = new CriarPedidoRequest(
                3001,
                4001,
                [new ItemPedidoRequest(5001, 2, 150m)]
            );

            // Act
            var response = await _client.PostAsJsonAsync(RotasApi.Pedidos.Rota, request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var content = await response.Content.ReadFromJsonAsync<CriarPedidoResponse>();
            content.Should().NotBeNull();
            content!.PedidoId.Should().Be(3001);
            content.Status.Should().Be(StatusEnum.Criado.ToString());
            
            // Verificar a URL de localização
            response.Headers.Location.Should().NotBeNull();
            response.Headers.Location!.ToString().Should().Contain($"{RotasApi.Pedidos.Rota}/{content.PedidoId}");
        }

        [Fact]
        public async Task GetPedidoPorId_DeveRetornarPedido_QuandoPedidoExiste()
        {
            // Arrange
            var criarRequest = new CriarPedidoRequest(
                3002,
                4002,
                [new ItemPedidoRequest(5002, 3, 200m)]
            );

            await _client.PostAsJsonAsync(RotasApi.Pedidos.Rota, criarRequest);

            // Act
            var response = await _client.GetAsync($"{RotasApi.Pedidos.Rota}/3002");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var content = await response.Content.ReadFromJsonAsync<ConsultarPedidoResponse>();
            content.Should().NotBeNull();
            content!.PedidoId.Should().Be(3002);
            content.ClienteId.Should().Be(4002);
            content.Status.Should().Be(StatusEnum.Criado.ToString());
            content.Itens.Should().HaveCount(1);
            content.Itens[0].ProdutoId.Should().Be(5002);
            content.Itens[0].Quantidade.Should().Be(3);
            content.Itens[0].Valor.Should().Be(200m);
        }

        [Fact]
        public async Task ListarPedidos_DeveRetornarPedidosFiltrados_QuandoStatusFornecido()
        {
            // Arrange
            var criarRequest1 = new CriarPedidoRequest(
                3003,
                4003,
                [new ItemPedidoRequest(5003, 1, 100m)]
            );

            var criarRequest2 = new CriarPedidoRequest(
                3004,
                4004,
                [new ItemPedidoRequest(5004, 1, 100m)]
            );

            await _client.PostAsJsonAsync(RotasApi.Pedidos.Rota, criarRequest1);
            await _client.PostAsJsonAsync(RotasApi.Pedidos.Rota, criarRequest2);

            // Act
            var response = await _client.GetAsync($"{RotasApi.Pedidos.Rota}?status=Criado");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var content = await response.Content.ReadFromJsonAsync<List<ConsultarPedidoResponse>>();
            content.Should().NotBeNull();
            
            // Verificar que os pedidos criados estão na lista
            content.Should().Contain(p => p.PedidoId == 3003);
            content.Should().Contain(p => p.PedidoId == 3004);
        }
    }
}