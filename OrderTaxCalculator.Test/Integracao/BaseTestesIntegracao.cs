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
        // Clean up the database after each test
        DbContext.Pedidos.RemoveRange(DbContext.Pedidos);
        DbContext.PedidoItens.RemoveRange(DbContext.PedidoItens);
        DbContext.SaveChanges();
    }
}