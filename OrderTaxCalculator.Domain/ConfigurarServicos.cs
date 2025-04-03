using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using OrderTaxCalculator.Domain.Interfaces.Servicos;
using OrderTaxCalculator.Domain.Servicos;
using OrderTaxCalculator.Domain.Strategy;

namespace OrderTaxCalculator.Domain;

public static class ConfigurarServicos
{
    public static void ConfigureServicos(this IServiceCollection service)
    {
        service.AddScoped<IPedidoServico, PedidoServico>();
        service.AddScoped<ICalcularImpostoServico, CalcularImpostoServicoStrategy>();

        service.AddFeatureManagement();
    }
}