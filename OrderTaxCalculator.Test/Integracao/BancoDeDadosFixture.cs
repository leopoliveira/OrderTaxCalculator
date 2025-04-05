using Microsoft.EntityFrameworkCore;
using OrderTaxCalculator.Data.BancoDeDados;
using Testcontainers.MsSql;

namespace OrderTaxCalculator.Test.Integracao;

public class BancoDeDadosFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _sqlContainer;
    public PedidoDbContext DbContext { get; private set; }
    
    public BancoDeDadosFixture()
    {
        _sqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("aPassword123@$")
            .WithCleanUp(true)
            .Build();
    }
    
    public async Task InitializeAsync()
    {
        // Start the SQL Server container
        await _sqlContainer.StartAsync();

        // Create and configure the DbContext
        var options = new DbContextOptionsBuilder<PedidoDbContext>()
            .UseSqlServer(_sqlContainer.GetConnectionString())
            .Options;

        DbContext = new PedidoDbContext(options);
            
        // Ensure database is created and migrations are applied
        await DbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await DbContext.DisposeAsync();
        await _sqlContainer.DisposeAsync();
    }
}