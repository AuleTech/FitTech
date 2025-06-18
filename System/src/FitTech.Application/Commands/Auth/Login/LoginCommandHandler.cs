using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Auth;
using FitTech.Application.Auth.Providers;
using FitTech.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FitTech.Application.Commands.Auth.Login;

internal sealed class LoginCommandHandler : IAuleTechCommandHandler<LoginCommand, Result<LoginResultDto>>
{
    private readonly UserManager<FitTechUser> _userManager;
    private readonly ILogger<LoginCommandHandler> _logger;
    private readonly ITokenProvider _tokenProvider;

    public LoginCommandHandler(UserManager<FitTechUser> userManager, ILogger<LoginCommandHandler> logger, ITokenProvider tokenProvider)
    {
        _userManager = userManager;
        _logger = logger;
        _tokenProvider = tokenProvider;
    }

    public async Task<Result<LoginResultDto>> HandleAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(command.Email).WaitAsync(cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("User('{Email}') not found", command.Email);
            return Result<LoginResultDto>.Failure();
        }

        var result = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash!, command.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            _logger.LogWarning("Wrong Password for User('{Email}')", command.Email);
            return Result<LoginResultDto>.Failure();
        }

        var accessToken = _tokenProvider.GenerateAccessToken(user);
        var refreshToken = await GetRefreshTokenAsync();

        return new LoginResultDto(accessToken, refreshToken);

        async Task<string> GetRefreshTokenAsync()
        {
            var existingRefreshToken = await _userManager
                .GetAuthenticationTokenAsync(user, TokenOptions.DefaultProvider, TokenType.Refresh).WaitAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(existingRefreshToken))
            {
                return await GenerateAndCreateRefreshTokenAsync();
            }

            var isValid = await _userManager
                .VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, TokenType.Refresh, existingRefreshToken)
                .WaitAsync(cancellationToken);

            if (!isValid)
            {
                await _userManager.RemoveAuthenticationTokenAsync(user, TokenOptions.DefaultProvider, TokenType.Refresh);
                return await GenerateAndCreateRefreshTokenAsync();
            }

            return existingRefreshToken;
        }

        async Task<string> GenerateAndCreateRefreshTokenAsync()
        {
            var newRefreshToken = await _userManager
                .GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, TokenType.Refresh)
                .WaitAsync(cancellationToken);
            await _userManager.SetAuthenticationTokenAsync(user, "FitTech", TokenType.Refresh, newRefreshToken)
                .WaitAsync(cancellationToken);
            return newRefreshToken;
        }
    }
}
