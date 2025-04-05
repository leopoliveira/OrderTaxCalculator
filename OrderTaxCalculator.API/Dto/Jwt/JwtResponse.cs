namespace OrderTaxCalculator.API.Dto.Jwt;

public record JwtResponse(string Token, DateTime ExpiresAt, string Type);