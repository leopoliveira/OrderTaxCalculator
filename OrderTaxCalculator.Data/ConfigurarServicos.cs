using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderTaxCalculator.Data.BancoDeDados;
using OrderTaxCalculator.Data.Constantes;
using OrderTaxCalculator.Data.Repositorios;
using OrderTaxCalculator.Domain.Interfaces.Repositorios;

namespace OrderTaxCalculator.Data;

public static class ConfigurarServicos
{
    public static void ConfigurePedidoDbContext(this IServiceCollection service)
    {
        const string bancoDeDados = ConstantesData.BancoDeDadosEmMemoria;
        
        service.AddDbContext<PedidoDbContext>(options =>
            options.UseInMemoryDatabase(bancoDeDados));
    }

    public static void ConfigureRepositorios(this IServiceCollection service)
    {
        service.AddScoped<IPedidoRepositorio, RepositorioPedido>();
    }
}