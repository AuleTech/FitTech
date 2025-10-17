using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.CQRS.Validations;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Extensions;

namespace FitTech.Application.Query.Auth.RefreshToken;

public record RefreshTokenQuery(string RefreshToken, string ExpiredAccessToken) : IValidator, IQuery
{
    public Result Validate()
    {
        var errors = new List<string>();
        RefreshToken.ValidateStringNullOrEmpty(errors, nameof(RefreshToken));
        ExpiredAccessToken.ValidateStringNullOrEmpty(errors, nameof(ExpiredAccessToken));

        return errors.Any() ? Result.Failure(errors) : Result.Success;
    }
}

public record RefreshTokenResultDto(string? AccessToken, bool NeedLoginAgain);
