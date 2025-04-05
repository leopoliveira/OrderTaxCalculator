using Microsoft.Extensions.Configuration;
using OrderTaxCalculator.API.Autenticacao;
using OrderTaxCalculator.API.Autenticacao.Servicos;
using OrderTaxCalculator.API.Constantes;

namespace OrderTaxCalculator.Test.Servicos;

public class JwtServicoTestes
{
    private readonly JwtServico _jwtServico;

    public JwtServicoTestes()
    {
        var configuracoesEmMemoria = new Dictionary<string, string> {
            {"JwtSettings:SecretKey", "chave_secreta_para_testes_com_tamanho_minimo_necessario_223266"},
            {"JwtSettings:Issuer", "TestIssuer"},
            {"JwtSettings:Audience", "TestAudience"},
            {"JwtSettings:ExpiryInMinutes", "60"}
        };
        
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configuracoesEmMemoria)
            .Build();

        var configuracoesJwt = new ConfiguracoesJwt();
        configuration.GetSection(ConstantesApi.ConfiguracaoJwtAmbiente).Bind(configuracoesJwt);
        
        _jwtServico = new JwtServico(configuracoesJwt);
    }

    [Fact]
    public void GereToken_ComClienteId_DeveRetornarToken()
    {
        // Arrange
        var clienteId = "test-client";

        // Act
        var token = _jwtServico.GereToken(clienteId);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }
}