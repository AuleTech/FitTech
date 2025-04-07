using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Unicode;
using FitTech.Application.Auth.Configuration;
using FitTech.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace FitTech.Application.Auth.Providers;

internal sealed class TokenProvider : ITokenProvider
{
    private readonly AuthenticationSettings _authenticationSettings;
    private readonly ClaimsPrincipal _anonymous = new (); 
    public TokenProvider(AuthenticationSettings authenticationSettings)
    { 
        _authenticationSettings = authenticationSettings;
    }

    public string GenerateAccessToken(FitTechUser user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.SigningKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject =
                new ClaimsIdentity([
                    new Claim(ClaimTypes.Email, user.Email!),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, "Trainer") //TODO: This needs to change depending on the user
                ]),
            Expires = DateTime.UtcNow.Add(_authenticationSettings.AccessTokenExpirationTime),
            SigningCredentials = credentials,
            Issuer = _authenticationSettings.Issuer,
            Audience = _authenticationSettings.Audience
        };

        JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();
        var handler = new JwtSecurityTokenHandler();
        var result =  handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(result);
    }

    public string GenerateRefreshToken()
    {
        var random = new Random();
        var buffer = new byte[32];
        random.NextBytes(buffer);
        return Convert.ToBase64String(buffer);
    }

    public ClaimsPrincipal GetClaimsPrincipalFromAccessToken(string accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return _anonymous;
        }

        var handler = new JwtSecurityTokenHandler();

        if (!handler.CanReadToken(accessToken))
        {
            throw new InvalidOperationException("Invalid token");
        }

        var securityToken = handler.ReadJwtToken(accessToken);

        return new ClaimsPrincipal([new ClaimsIdentity(securityToken.Claims)]);
    }
}
