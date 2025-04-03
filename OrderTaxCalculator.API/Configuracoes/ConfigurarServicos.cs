using Microsoft.AspNetCore.Http.Features;
using OrderTaxCalculator.API.Errors;

namespace OrderTaxCalculator.API.Configuracoes;

public static class ConfigurarServicos
{
    public static void ConfigureServicosApi(this IServiceCollection service)
    {
        service.AddProblemDetails(
            options =>
            {
                options.CustomizeProblemDetails = context =>
                {
                    context.ProblemDetails.Instance =
                        $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

                    context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

                    var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                    context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
                };
            }
        );
        service.AddExceptionHandler<ProblemsExceptionsHandler>();
    }
}