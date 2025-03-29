using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderTaxCalculator.Data.Constants;
using OrderTaxCalculator.Data.Database;

namespace OrderTaxCalculator.Data;

public static class ConfigurarServicos
{
    public static void ConfigurePedidoDbContext(this IServiceCollection service)
    {
        const string bancoDeDados = DataConstants.BancoDeDadosEmMemoria;
        
        service.AddDbContext<PedidoDbContext>(options =>
            options.UseInMemoryDatabase(bancoDeDados));
    }
}