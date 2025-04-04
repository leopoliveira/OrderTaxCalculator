using OrderTaxCalculator.Domain.Strategy;

namespace OrderTaxCalculator.Test.Servicos.ClassesStrategy;

public class CalcularImpostoServicoStrategyTestes
{
    private readonly CalcularImpostoServicoStrategy _strategy;

    public CalcularImpostoServicoStrategyTestes()
    {
        _strategy = new CalcularImpostoServicoStrategy();
    }

    [Fact]
    public void CalcularImposto_DeveRetornarZero_QuandoTotalItensForZero()
    {
        // Arrange
        decimal totalItens = 0;

        // Act
        var resultado = _strategy.CalcularImposto(totalItens);

        // Assert
        resultado.Should().Be(0);
    }

    [Fact]
    public void CalcularImposto_DeveCalcularCorretamente_ComTaxaPadrao()
    {
        // Arrange
        decimal totalItens = 100;
        decimal taxaEsperada = 0.3M;

        // Act
        var resultado = _strategy.CalcularImposto(totalItens);

        // Assert
        resultado.Should().Be(totalItens * taxaEsperada);
        resultado.Should().Be(30);
    }

    [Theory]
    [InlineData(100, 30)]
    [InlineData(0, 0)]
    [InlineData(50.5, 15.15)]
    [InlineData(999.99, 299.997)]
    [InlineData(1234.56, 370.368)]
    public void CalcularImposto_DeveCalcularCorretamente_ParaDiversosValores(decimal totalItens, decimal impostoEsperado)
    {
        // Act
        var resultado = _strategy.CalcularImposto(totalItens);

        // Assert
        resultado.Should().BeApproximately(impostoEsperado, 0.001M);
    }

    [Fact]
    public void CalcularImposto_DeveManterPrecisaoDecimal_QuandoCalculoResultaEmMultiplasDecimais()
    {
        // Arrange
        decimal totalItens = 333.33M;
        decimal resultadoEsperado = 99.999M;

        // Act
        var resultado = _strategy.CalcularImposto(totalItens);

        // Assert
        resultado.Should().BeApproximately(resultadoEsperado, 0.001M);
    }

    [Fact]
    public void CalcularImposto_DeveCalcularCorretamente_ParaValoresMaximos()
    {
        // Arrange
        decimal totalItens = 1000000M;
        decimal resultadoEsperado = 300000M;

        // Act
        var resultado = _strategy.CalcularImposto(totalItens);

        // Assert
        resultado.Should().Be(resultadoEsperado);
    }

    [Fact]
    public void CalcularImposto_DeveCalcularCorretamente_ParaValoresNegativos()
    {
        // Arrange
        decimal totalItens = -100M;
        decimal resultadoEsperado = -30M;

        // Act
        var resultado = _strategy.CalcularImposto(totalItens);

        // Assert
        resultado.Should().Be(resultadoEsperado);
    }
}