using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using OrderTaxCalculator.Domain.Interfaces.Services;
using OrderTaxCalculator.Domain.Services;
using OrderTaxCalculator.Domain.Strategy;

namespace OrderTaxCalculator.Domain;

public static class ConfigurarServicos
{
    public static void ConfigureServicos(this IServiceCollection service)
    {
        service.AddScoped<IPedidoService, PedidoService>();
        service.AddScoped<ICalcularImpostoService, CalcularImpostoServiceStrategy>();

        service.AddFeatureManagement();
    }
}