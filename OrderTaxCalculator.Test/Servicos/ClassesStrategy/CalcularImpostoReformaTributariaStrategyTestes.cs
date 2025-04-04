using OrderTaxCalculator.Domain.Strategy;

namespace OrderTaxCalculator.Test.Servicos.ClassesStrategy;

public class CalcularImpostoReformaTributariaStrategyTestes
{
    private readonly CalcularImpostoReformaTributariaStrategy _strategy;

    public CalcularImpostoReformaTributariaStrategyTestes()
    {
        _strategy = new CalcularImpostoReformaTributariaStrategy();
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
    public void CalcularImposto_DeveCalcularCorretamente_ComTaxaReformaTributaria()
    {
        // Arrange
        decimal totalItens = 100;
        decimal taxaEsperada = 0.2M;

        // Act
        var resultado = _strategy.CalcularImposto(totalItens);

        // Assert
        resultado.Should().Be(totalItens * taxaEsperada);
        resultado.Should().Be(20);
    }

    [Theory]
    [InlineData(100, 20)]
    [InlineData(0, 0)]
    [InlineData(50.5, 10.1)]
    [InlineData(999.99, 199.998)]
    [InlineData(1234.56, 246.912)]
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
        decimal resultadoEsperado = 66.666M;

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
        decimal resultadoEsperado = 200000M;

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
        decimal resultadoEsperado = -20M;

        // Act
        var resultado = _strategy.CalcularImposto(totalItens);

        // Assert
        resultado.Should().Be(resultadoEsperado);
    }

    [Fact]
    public void CalcularImposto_DeveProduzirResultadoDiferenteDoImpostoServicoStrategy()
    {
        // Arrange
        decimal totalItens = 100M;
        var impostoServicoStrategy = new CalcularImpostoServicoStrategy();
        var impostoReformaTributariaStrategy = new CalcularImpostoReformaTributariaStrategy();

        // Act
        var resultadoImpostoServico = impostoServicoStrategy.CalcularImposto(totalItens);
        var resultadoReformaTributaria = impostoReformaTributariaStrategy.CalcularImposto(totalItens);

        // Assert
        resultadoImpostoServico.Should().NotBe(resultadoReformaTributaria);
        resultadoImpostoServico.Should().Be(30M);
        resultadoReformaTributaria.Should().Be(20M);
    }
}