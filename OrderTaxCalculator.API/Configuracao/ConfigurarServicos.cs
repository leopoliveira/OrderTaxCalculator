using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrderTaxCalculator.API.Autenticacao;
using OrderTaxCalculator.API.Autenticacao.Interfaces;
using OrderTaxCalculator.API.Autenticacao.Servicos;
using OrderTaxCalculator.API.Constantes;

namespace OrderTaxCalculator.API.Configuracao;

public static class ConfigurarServicos
{
    public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var configuracoesJwt = new ConfiguracoesJwt();
        configuration.GetSection(ConstantesApi.ConfiguracaoJwtAmbiente).Bind(configuracoesJwt);
        
        services.AddSingleton(configuracoesJwt);
        services.AddSingleton<IJwtServico, JwtServico>();
        
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuracoesJwt.Issuer,
                ValidAudience = configuracoesJwt.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuracoesJwt.SecretKey))
            };
            options.Events = new JwtBearerEvents
            {
                OnChallenge = async context =>
                {
                    context.HandleResponse();
                    
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/problem+json";

                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Title = "Não autorizado",
                        Detail = "Token de autenticação ausente, inválido ou expirado",
                        Instance = context.Request.Path,
                    };

                    await context.Response.WriteAsJsonAsync(problemDetails);
                }
            };
        });

        services.AddAuthorization();
    }

    public static void ConfigureDocumentacaoSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "OrderTaxCalculator", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Enviei o Token JWT usando o header. Exemplo: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    []
                }
            });
        });
    }
}