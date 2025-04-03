using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderTaxCalculator.Data.Constants;
using OrderTaxCalculator.Data.Database;
using OrderTaxCalculator.Data.Repositories;
using OrderTaxCalculator.Domain.Interfaces.Repositories;

namespace OrderTaxCalculator.Data;

public static class ConfigurarServicos
{
    public static void ConfigurePedidoDbContext(this IServiceCollection service)
    {
        const string bancoDeDados = DataConstants.BancoDeDadosEmMemoria;
        
        service.AddDbContext<PedidoDbContext>(options =>
            options.UseInMemoryDatabase(bancoDeDados));
    }

    public static void ConfigureRepositorios(this IServiceCollection service)
    {
        service.AddScoped<IPedidoRepository, PedidoRepository>();
    }
}