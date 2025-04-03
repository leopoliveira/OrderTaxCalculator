using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace OrderTaxCalculator.API.Errors;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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