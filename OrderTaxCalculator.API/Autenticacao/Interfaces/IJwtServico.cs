namespace OrderTaxCalculator.API.Autenticacao.Interfaces;

public interface IJwtServico
{
    string GereToken(string clienteId);
}