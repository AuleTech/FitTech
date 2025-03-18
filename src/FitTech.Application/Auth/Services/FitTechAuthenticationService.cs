using System.Web;
using FitTech.Application.Auth.Dtos;
using FitTech.Application.Auth.Providers;
using FitTech.Application.Extensions;
using FitTech.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FitTech.Application.Auth.Services;

internal sealed class FitTechAuthenticationService : IFitTechAuthenticationService
{
    private readonly ILogger<FitTechAuthenticationService> _logger;
    private readonly ITokenProvider _tokenProvider;
    private readonly UserManager<FitTechUser> _userManager;

    public FitTechAuthenticationService(UserManager<FitTechUser> userManager,
        ILogger<FitTechAuthenticationService> logger, ITokenProvider tokenProvider)
    {
        _userManager = userManager;
        _logger = logger;
        _tokenProvider = tokenProvider;
    }

    public async Task<Result> RegisterAsync(RegisterUserDto registerUserDto, CancellationToken cancellationToken)
    {
        var result = await _userManager.CreateAsync(registerUserDto.MapToIdentityUser(), registerUserDto.Password)
            .WaitAsync(cancellationToken);

        return result.ToResult();
    }

    public async Task<Result<LoginResultDto>> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email).WaitAsync(cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("User('{Email}') not found", loginDto.Email);
            return Result<LoginResultDto>.Failure();
        }

        var result = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash!, loginDto.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            _logger.LogWarning("Wrong Password for User('{Email}')", loginDto.Email);
            return Result<LoginResultDto>.Failure();
        }

        var token = _tokenProvider.Create(user);
        //TODO: Return RefreshToken

        return new LoginResultDto(token);
    }

    public async Task<Result<string>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(forgotPasswordDto.Email))
        {
            _logger.LogError("Email is required to recover password");
            return Result<string>.Failure(["Email is required to recover password"]);
        }

        var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email).WaitAsync(cancellationToken);

        if (user is null)
        {
            _logger.LogError("User not found, returning Ok to avoid getting mail register information");
            return Result<string>.Success(string.Empty);
        }

        var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user).WaitAsync(cancellationToken); //TODO: We need to normalized for http.
        
        
        //TODO: Remove this log when we send the email
        //CallbackUrl url/resetpassword
        _logger.LogInformation("ResetPasswordUrl: {Url}",$"{forgotPasswordDto.CallbackUrl}?email={forgotPasswordDto.Email}&token={resetPasswordToken}");

        return Result<string>.Success(HttpUtility.UrlEncode(resetPasswordToken));
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordDto resetPasswordDto, CancellationToken cancellationToken)
    {
        //TODO: DtoValidation
        var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email).WaitAsync(cancellationToken);

        if (user is null)
        {
            _logger.LogError("User not found{Email}", resetPasswordDto.Email);
            return Result.Failure(["Something went wrong"]);
        }
        
        var result = await _userManager.ResetPasswordAsync(user, HttpUtility.HtmlDecode(resetPasswordDto.Token), resetPasswordDto.Password);
        
        return result.ToResult();
    }
    
}
