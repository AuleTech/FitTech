using System.Security.Claims;
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
    private readonly IEmailService _emailService;

    public FitTechAuthenticationService(UserManager<FitTechUser> userManager,
        ILogger<FitTechAuthenticationService> logger, ITokenProvider tokenProvider, IEmailService emailService)
    {
        _userManager = userManager;
        _logger = logger;
        _tokenProvider = tokenProvider;
        _emailService = emailService;
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

    public async Task<Result<string>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(forgotPasswordDto.Email))
        {
            _logger.LogError("Email is required to recover password");
            return Result<string>.Failure("Email is required to recover password");
        }

        var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email).WaitAsync(cancellationToken);

        if (user is null)
        {
            _logger.LogError("User not found, returning Ok to avoid getting mail register information");
            return Result<string>.Success(string.Empty);
        }

        var resetPasswordToken =
            await _userManager.GeneratePasswordResetTokenAsync(user)
                .WaitAsync(cancellationToken); //TODO: We need to normalized for http.
        
        //Creamos Url con token.
        var encodedToken = HttpUtility.UrlEncode(resetPasswordToken);
        var callbackUrl = $"{forgotPasswordDto.CallbackUrl}?email={forgotPasswordDto.Email}&token={encodedToken}";
        
        // TODO: Hacer una platilla real.
        var emailBody = $"<p>Click <a href='{callbackUrl}'>here</a> to reset your password.</p>";
        
        //Llamamos a la funcion Enviar email
        await _emailService.SendEmailAsync(
            forgotPasswordDto.Email,
            "Reset your FitTech password",
            emailBody
        );
        
        //TODO: Remove this log when we send the email
        //CallbackUrl url/resetpassword
        _logger.LogInformation("ResetPasswordUrl: {Url}",
            $"{forgotPasswordDto.CallbackUrl}?email={forgotPasswordDto.Email}&token={resetPasswordToken}");

        return Result<string>.Success(HttpUtility.UrlEncode(resetPasswordToken));
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordDto resetPasswordDto, CancellationToken cancellationToken)
    {
        //TODO: DtoValidation
        var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email).WaitAsync(cancellationToken);

        if (user is null)
        {
            _logger.LogError("User not found('{Email}')", resetPasswordDto.Email);
            return Result.Failure("Something went wrong");
        }

        var result = await _userManager.ResetPasswordAsync(user, HttpUtility.HtmlDecode(resetPasswordDto.Token),
            resetPasswordDto.Password);

        return result.ToResult();
    }

    public async Task<Result<RefreshTokenResultDto>> RefreshTokenAsync(RefreshTokenDto refreshTokenDto, CancellationToken cancellationToken)
    {
       var validationResult =  refreshTokenDto.Validate();

       if (!validationResult.Succeeded)
       {
           return validationResult.ToTypedResult<RefreshTokenResultDto>();
       }
        
       var claimsPrincipal = _tokenProvider.GetClaimsPrincipalFromAccessToken(refreshTokenDto.ExpiredAccessToken);

        var userEmail = claimsPrincipal.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Email);

        if (userEmail is null)
        {
            return Result<RefreshTokenResultDto>.Failure($"Token missing claim: {ClaimTypes.Email}");
        }

        var user = await _userManager.FindByEmailAsync(userEmail.Value).WaitAsync(cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("Token was provided and validated but the user doesn't exists. Needs to be investigated!");
            return Result<RefreshTokenResultDto>.Failure();
        }

        var isValid = await _userManager
            .VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, TokenType.Refresh, refreshTokenDto.RefreshToken)
            .WaitAsync(cancellationToken);

        if (!isValid)
        {
            _logger.LogWarning("Need to reintroduce credentials");
            return new RefreshTokenResultDto(string.Empty, true);
        }

        return new RefreshTokenResultDto(_tokenProvider.GenerateAccessToken(user), false);
    }
}
