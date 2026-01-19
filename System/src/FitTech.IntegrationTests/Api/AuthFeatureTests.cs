using AuleTech.Core.Resiliency;
using AwesomeAssertions;
using FitTech.ApiClient.Generated;
using FitTech.Domain.Enums;
using FitTech.TestingSupport;
using FitTech.TestingSupport.Assertions;
using Microsoft.EntityFrameworkCore;

namespace FitTech.IntegrationTests.Api;

public class AuthFeatureTests
{
    [ClassDataSource<TestHost>(Shared = SharedType.PerTestSession)]
    public required TestHost Host { get; init; }

    [Test]
    public async Task Auth_ResetPasswordFlow_CanLoginWithNewPassword()
    {
        var testCredentials = await Host.ApiClient.GetTestTrainerCredentialsAsync(CancellationToken.None);
        
        var forgotPasswordRequest = new ForgotPasswordRequest()
        {
          Email  = testCredentials.Email,
          CallbackUrl = "www.url.com"
        };

        var response = await Host.ApiClient.Auth.ForgotPasswordAsync(forgotPasswordRequest, CancellationToken.None); 
        response.IsSuccessful.Should().BeTrue();
        
        var token = await GetTokenFromEmailAsync();

        var resetPasswordRequest = new ResetPasswordRequest()
        {
            Email = forgotPasswordRequest.Email,
            Token = token,
            NewPassword = "NewPassword1234!"
        };

        var resetPasswordResponse = await Host.ApiClient.Auth.ResetPasswordAsync(resetPasswordRequest, CancellationToken.None);
        resetPasswordResponse.IsSuccessful.Should().BeTrue();
        
        
        var loginRequest = new LoginRequest()
        {
            Email = forgotPasswordRequest.Email,
            Password = resetPasswordRequest.NewPassword
        };

        var loginResult = await Host.ApiClient.Auth.LoginAsync(loginRequest, CancellationToken.None);
        loginResult.Assert();

        async Task<string> GetTokenFromEmailAsync()
        {
            var dbContext = await Host.GetFitTechApiDbContextAsync(CancellationToken.None);
            var emailLog = await dbContext.Emails.FirstAsync(x => x.EmailType == EmailType.ResetPassword, CancellationToken.None);
        
            var resendClient = FitTechTestingSupport.GetResendTestClient();

            var email = await ResilientOperations.Default.RetryIfNeededAsync(async (_) =>
                {
                    var email = await resendClient.EmailRetrieveAsync(emailLog.ExternalId, CancellationToken.None);

                    return email;
                }, waitBetweenAttempts: TimeSpan.FromSeconds(1),
                maxAttempts: 5);
        
            email.Content.HtmlBody.Should().Contain("token=");
        
            var s = FitTechEmailTestExtensions.GetForgotPasswordTokenFromEmailBody(email.Content.HtmlBody);
            s.Should().NotBeNullOrWhiteSpace();
            return s;
        }
    }
}
