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
        // Inicia o SQL Server container
        await _sqlContainer.StartAsync();

        // Crie e configure o DbContext
        var options = new DbContextOptionsBuilder<PedidoDbContext>()
            .UseSqlServer(_sqlContainer.GetConnectionString())
            .Options;

        DbContext = new PedidoDbContext(options);
            
        // Garanta a base crida
        await DbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await DbContext.DisposeAsync();
        await _sqlContainer.DisposeAsync();
    }
}