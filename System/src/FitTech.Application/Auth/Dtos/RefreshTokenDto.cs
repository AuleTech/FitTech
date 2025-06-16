using AuleTech.Core.Patterns;
using FitTech.Application.Extensions;

namespace FitTech.Application.Auth.Dtos;

public record RefreshTokenDto(string RefreshToken, string ExpiredAccessToken) : SelfValidatedDto
{
    public override Result Validate()
    {
        if (string.IsNullOrWhiteSpace(RefreshToken))
        {
            return Result.Failure(RefreshToken.RequiredErrorMessage());
        }

        if (string.IsNullOrWhiteSpace(ExpiredAccessToken))
        {
            return Result.Failure(ExpiredAccessToken.RequiredErrorMessage());
        }
        
        return Result.Success;
    }
}

public record RefreshTokenResultDto(string? AccessToken, bool NeedLoginAgain);
