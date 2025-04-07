using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FitTech.WebComponents.Models;

public class FitTechUser
{
    public static string StorageKey => nameof(FitTechUser);
    public string Email { get; set; } = null!;
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }

    internal ClaimsPrincipal GetClaimsPrincipal()
    {
        if (string.IsNullOrWhiteSpace(AccessToken))
        {
            return new ClaimsPrincipal();
        }
        
        var tokenHandler = new JwtSecurityTokenHandler();
        tokenHandler.InboundClaimTypeMap.Clear();
        var claims = new ClaimsIdentity("FitTechAuth");

        if (tokenHandler.CanReadToken(AccessToken))
        {
            var securityToken = tokenHandler.ReadJwtToken(AccessToken);
            claims.AddClaims(securityToken.Claims);
        }

        return new ClaimsPrincipal(claims);
    }
}

