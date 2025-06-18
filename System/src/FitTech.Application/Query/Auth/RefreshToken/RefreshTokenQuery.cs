using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Auth.Dtos;
using FitTech.Application.Extensions;

namespace FitTech.Application.Query.Auth.RefreshToken;

public record RefreshTokenQuery(string RefreshToken, string ExpiredAccessToken) : SelfValidatedDto, IQuery 
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
