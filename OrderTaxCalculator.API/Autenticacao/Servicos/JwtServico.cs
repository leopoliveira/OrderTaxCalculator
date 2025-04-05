using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OrderTaxCalculator.API.Autenticacao.Interfaces;

namespace OrderTaxCalculator.API.Autenticacao.Servicos;

public class JwtServico : IJwtServico
{
    private readonly ConfiguracoesJwt _configuracoesJwt;

    public JwtServico(ConfiguracoesJwt configuracoesJwt)
    {
        _configuracoesJwt = configuracoesJwt;
    }

    public string GereToken(string clienteId)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuracoesJwt.SecretKey));
        var credenciais = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, clienteId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuracoesJwt.Issuer,
            audience: _configuracoesJwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_configuracoesJwt.ExpiryInMinutes),
            signingCredentials: credenciais
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}