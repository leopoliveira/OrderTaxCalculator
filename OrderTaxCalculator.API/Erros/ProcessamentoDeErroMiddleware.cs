using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace OrderTaxCalculator.API.Erros;

public class ProcessamentoDeErroMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ProcessamentoDeErroMiddleware> _logger;

    public ProcessamentoDeErroMiddleware(RequestDelegate next, ILogger<ProcessamentoDeErroMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado");
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var problem = new ProblemDetails
        {
            Title = "Erro interno no servidor",
            Detail = exception.Message,
            Status = (int)HttpStatusCode.InternalServerError
        };
        
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = problem.Status.Value;
        
        return context.Response.WriteAsJsonAsync(problem);
    }
}