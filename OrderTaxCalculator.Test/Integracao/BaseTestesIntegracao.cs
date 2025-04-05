using OrderTaxCalculator.Data.BancoDeDados;

namespace OrderTaxCalculator.Test.Integracao;

public abstract class BaseTestesIntegracao : IClassFixture<BancoDeDadosFixture>, IDisposable
{
    protected readonly BancoDeDadosFixture Fixture;
    protected PedidoDbContext DbContext => Fixture.DbContext;
    
    protected BaseTestesIntegracao(BancoDeDadosFixture fixture)
    {
        Fixture = fixture;
    }
    
    public void Dispose()
    {
        // Limpa a base a cada teste.
        DbContext.Pedidos.RemoveRange(DbContext.Pedidos);
        DbContext.PedidoItens.RemoveRange(DbContext.PedidoItens);
        DbContext.SaveChanges();
    }
}