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
        var problemDetails = new ProblemDetails();

        switch (exception)
        {
            case UnauthorizedAccessException:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                problemDetails.Status = StatusCodes.Status401Unauthorized;
                problemDetails.Title = "Não autorizado";
                problemDetails.Detail = "Você não está autorizado a acessar este recurso";
                break;
                
            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "Erro interno do servidor";
                problemDetails.Detail = "Ocorreu um erro ao processar sua solicitação";
                break;
        }
        
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = problemDetails.Status.Value;
        
        return context.Response.WriteAsJsonAsync(problemDetails);
    }
}