using Microsoft.AspNetCore.Mvc;

namespace OrderTaxCalculator.API.Errors;

public class ApiError(string Erro) : ProblemDetails;