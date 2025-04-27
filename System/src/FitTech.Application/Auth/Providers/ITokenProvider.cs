using System.Security.Claims;
using FitTech.Domain.Entities;

namespace FitTech.Application.Auth.Providers;

internal interface ITokenProvider
{
    string GenerateAccessToken(FitTechUser user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetClaimsPrincipalFromAccessToken(string accessToken);
}
