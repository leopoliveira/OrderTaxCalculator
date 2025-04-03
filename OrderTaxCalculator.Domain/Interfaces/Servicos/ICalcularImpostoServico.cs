namespace OrderTaxCalculator.Domain.Interfaces.Servicos;

public interface ICalcularImpostoServico
{
    decimal CalcularImposto(decimal totalItens);
}