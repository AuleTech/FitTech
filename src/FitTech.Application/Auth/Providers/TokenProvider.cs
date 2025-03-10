using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FitTech.Application.Auth.Configuration;
using FitTech.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace FitTech.Application.Auth.Providers;

internal sealed class TokenProvider : ITokenProvider
{
    private readonly AuthenticationSettings _authenticationSettings;

    public TokenProvider(AuthenticationSettings authenticationSettings)
    {
        _authenticationSettings = authenticationSettings;
    }

    public string Create(FitTechUser user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.SigningKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject =
                new ClaimsIdentity([
                    new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())
                ]),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = credentials,
            Issuer = _authenticationSettings.Issuer,
            Audience = _authenticationSettings.Audience
        };

        var handler = new JwtSecurityTokenHandler();
        
        var result =  handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(result);
    }
}
