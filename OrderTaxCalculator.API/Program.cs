using OrderTaxCalculator.API.Configuracoes;
using OrderTaxCalculator.Data;
using OrderTaxCalculator.Domain;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigurePedidoDbContext();
builder.Services.ConfigureRepositorios();
builder.Services.ConfigureServicos();
builder.Services.ConfigureServicosApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();