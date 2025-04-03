namespace OrderTaxCalculator.Domain.Interfaces.Services;

public interface ICalcularImpostoService
{
    decimal CalcularImposto(decimal totalItens);
}