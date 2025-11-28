using System.Security.Claims;
using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Extensions;
using FitTech.Application.Providers;
using FitTech.Domain.Aggregates.AuthAggregate;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FitTech.Application.Query.Auth.RefreshToken;

public interface IRefreshTokenQueryHandler : IQueryHandler<RefreshTokenQuery, Result<RefreshTokenResultDto>>;
internal sealed class RefreshTokenQueryHandler : IRefreshTokenQueryHandler
{
    private readonly ILogger<RefreshTokenQuery> _logger;
    private readonly ITokenProvider _tokenProvider;
    private readonly UserManager<FitTechUser> _userManager;

    public RefreshTokenQueryHandler(UserManager<FitTechUser> userManager, ILogger<RefreshTokenQuery> logger,
        ITokenProvider tokenProvider)
    {
        _userManager = userManager;
        _logger = logger;
        _tokenProvider = tokenProvider;
    }

    public async Task<Result<RefreshTokenResultDto>> HandleAsync(RefreshTokenQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = query.Validate();

        if (!validationResult.Succeeded)
        {
            return validationResult.ToTypedResult<RefreshTokenResultDto>();
        }

        var claimsPrincipal = _tokenProvider.GetClaimsPrincipalFromAccessToken(query.ExpiredAccessToken);

        var userEmail = claimsPrincipal.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Email);

        if (userEmail is null)
        {
            return Result<RefreshTokenResultDto>.Failure($"Token missing claim: {ClaimTypes.Email}");
        }

        var user = await _userManager.FindByEmailAsync(userEmail.Value).WaitAsync(cancellationToken);

        if (user is null)
        {
            _logger.LogWarning(
                "Token was provided and validated but the user doesn't exists. Needs to be investigated!");
            return Result<RefreshTokenResultDto>.Failure();
        }

        var isValid = await _userManager
            .VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, TokenType.Refresh, query.RefreshToken)
            .WaitAsync(cancellationToken);

        if (!isValid)
        {
            _logger.LogWarning("Need to reintroduce credentials");
            throw new UnauthorizedAccessException("Need to reintroduce credentials");
        }

        return new RefreshTokenResultDto(_tokenProvider.GenerateAccessToken(user), false);
    }
}
