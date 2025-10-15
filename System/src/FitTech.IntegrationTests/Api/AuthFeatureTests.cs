using System.Net.Http.Headers;
using AuleTech.Core.Resiliency;
using AwesomeAssertions;
using FitTech.API.Endpoints.Auth.Login;
using FitTech.TestingSupport;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FitTech.IntegrationTests.Api;

public class AuthFeatureTests
{
    [ClassDataSource<TestHost>(Shared = SharedType.PerTestSession)]
    public required TestHost Host { get; init; }

    [Test]
    public async Task Auth_ResetPasswordFlow_CanLoginWithNewPassword()
    {
        var client = Host.GetClientApiClient();

        var forgotPasswordRequest = new ApiClient.ForgotPasswordRequest()
        {
          Email  = "admin@fittech.es",
          CallbackUrl = "www.thisisatest.com"
        };

        var response = await client.ForgotPasswordAsync(forgotPasswordRequest, CancellationToken.None); 
        response.Succeeded.Should().BeTrue();
        
        var token = await GetTokenFromEmailAsync();

        var resetPasswordRequest = new ApiClient.ResetPasswordRequest()
        {
            Email = forgotPasswordRequest.Email,
            Token = token,
            NewPassword = "NewPassword1234!"
        };

        var resetPasswordResponse = await client.ResetPasswordAsync(resetPasswordRequest, CancellationToken.None);
        resetPasswordResponse.Succeeded.Should().BeTrue();
        
        
        var loginRequest = new ApiClient.LoginRequest()
        {
            Email = forgotPasswordRequest.Email,
            Password = resetPasswordRequest.NewPassword
        };

        var loginResult = await client.LoginAsync(loginRequest, CancellationToken.None);
        loginResult.Assert();

        async Task<string> GetTokenFromEmailAsync()
        {
            var dbContext = await Host.GetFitTechApiDbContextAsync(CancellationToken.None);
            var emailLog = await dbContext.EmailLogs.FirstAsync(x => x.TypeMessage == "Reset Password", CancellationToken.None);
        
            var resendClient = FitTechTestingSupport.GetResendTestClient();

            var email = await ResilientOperations.Default.RetryIfNeededAsync(async (_) =>
                {
                    var email = await resendClient.EmailRetrieveAsync(emailLog.ExternalId, CancellationToken.None);

                    return email;
                }, waitBetweenAttempts: TimeSpan.FromMilliseconds(400),
                maxAttempts: 5);
        
            email.Content.HtmlBody.Should().Contain("token=");
        
            var s = FitTechEmailTestExtensions.GetForgotPasswordTokenFromEmailBody(email.Content.HtmlBody);
            s.Should().NotBeNullOrWhiteSpace();
            return s;
        }
    }
}
