using FitTech.Application.Auth.Dtos;
using FitTech.Application.Auth.Providers;
using FitTech.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FitTech.Application.Auth.Services;

internal sealed class FitTechAuthenticationService : IFitTechAuthenticationService
{
    private readonly ILogger<FitTechAuthenticationService> _logger;
    private readonly UserManager<FitTechUser> _userManager;
    private readonly ITokenProvider _tokenProvider;

    public FitTechAuthenticationService(UserManager<FitTechUser> userManager,
        ILogger<FitTechAuthenticationService> logger, ITokenProvider tokenProvider)
    {
        _userManager = userManager;
        _logger = logger;
        _tokenProvider = tokenProvider;
    }

    public async Task<IdentityResult> RegisterAsync(RegisterUserDto registerUserDto, CancellationToken cancellationToken)
    {
        var result = await _userManager.CreateAsync(registerUserDto.MapToIdentityUser(), registerUserDto.Password)
            .WaitAsync(cancellationToken);

        return result;
    }

    public async Task<LoginResultDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email).WaitAsync(cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("User('{Email}') not found", loginDto.Email);
            return LoginResultDto.Failed;   
        }
        
        var result = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash!, loginDto.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            _logger.LogWarning("Wrong Password for User('{Email}')", loginDto.Email);
            return LoginResultDto.Failed;
        }

        var token = _tokenProvider.Create(user);
        //TODO: Return RefreshToken

        return new LoginResultDto(true, token);
    }
}
