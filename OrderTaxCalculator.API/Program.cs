using OrderTaxCalculator.API.Configuracoes;
using OrderTaxCalculator.Data;
using OrderTaxCalculator.Domain;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog(
    (context, loggerConfiguration) =>
        loggerConfiguration.ReadFrom.Configuration(context.Configuration)
            .WriteTo.Console()
);

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigurePedidoDbContext();
builder.Services.ConfigureRepositorios();
builder.Services.ConfigureServicos();
builder.Services.ConfigureServicosApi();

var app = builder.Build();

app.UseSerilogRequestLogging();

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