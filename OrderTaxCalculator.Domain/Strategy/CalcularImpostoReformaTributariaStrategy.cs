using OrderTaxCalculator.Domain.Interfaces.Servicos;

namespace OrderTaxCalculator.Domain.Strategy;

public class CalcularImpostoReformaTributariaStrategy : ICalcularImpostoServico
{
    public decimal CalcularImposto(decimal totalItens) => totalItens * ObterTaxa();

    private static decimal ObterTaxa()
    {
        return 0.2M;
    }
}