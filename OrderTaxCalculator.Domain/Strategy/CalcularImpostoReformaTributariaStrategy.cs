using OrderTaxCalculator.Domain.Interfaces.Services;

namespace OrderTaxCalculator.Domain.Strategy;

public class CalcularImpostoReformaTributariaStrategy : ICalcularImpostoService
{
    public decimal CalcularImposto(decimal totalItens) => totalItens * ObterTaxa();

    private static decimal ObterTaxa()
    {
        return 0.2M;
    }
}