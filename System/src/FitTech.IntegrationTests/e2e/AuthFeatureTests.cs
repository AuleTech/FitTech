using System.Net.Http.Headers;
using System.Net.Http.Json;
using AuleTech.Core.Resiliency;
using AuleTech.Core.System.Http;
using AwesomeAssertions;
using FitTech.API.Endpoints.Auth.ForgotPassword;
using FitTech.API.Endpoints.Auth.Login;
using FitTech.API.Endpoints.Auth.ResetPassword;
using FitTech.TestingSupport;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FitTech.IntegrationTests.e2e;

public class AuthFeatureTests
{
    [ClassDataSource<TestHost>(Shared = SharedType.PerTestSession)]
    public required TestHost Host { get; init; }

    [Test]
    public async Task Auth_ResetPasswordFlow_CanLoginWithNewPassword()
    {
        var client = Host.GetClient();

        var forgotPasswordRequest = new ForgotPasswordRequest("admin@fittech.es", "www.thisisatest.com");
        var request = new StringContent(JsonConvert.SerializeObject(forgotPasswordRequest));
        request.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
        var response = await client.PostAsync("/auth/forgot-password", request, CancellationToken.None);
        
        response.StatusCode.IsSuccess().Should().BeTrue();
        
        var dbContext = await Host.GetFitTechApiDbContextAsync(CancellationToken.None);
        var emailLog = await dbContext.EmailLog.FirstAsync(x => x.TypeMessage == "Reset Password", CancellationToken.None);
        
        var resendClient = FitTechTestingSupport.GetResendTestClient();

        var email = await ResilientOperations.Default.RetryIfNeededAsync(async (_) =>
        {
            var email = await resendClient.EmailRetrieveAsync(emailLog.ExternalId, CancellationToken.None);

            return email;
        }, waitBetweenAttempts: TimeSpan.FromMilliseconds(100),
            maxAttempts: 5);
        
        email.Content.HtmlBody.Should().Contain("token=");
        
        var token = FitTechEmailExtensions.GetForgotPasswordTokenFromEmailBody(email.Content.HtmlBody);
        token.Should().NotBeNullOrWhiteSpace();
        
        var resetPasswordRequest = new ResetPasswordRequest(forgotPasswordRequest.Email, token, "NewPassword1234!");
        request = new StringContent(JsonConvert.SerializeObject(resetPasswordRequest));
        request.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

        response = await client.PostAsync("/auth/reset-password", request, CancellationToken.None);
        response.StatusCode.IsSuccess().Should().BeTrue();

        var login = new LoginRequest(forgotPasswordRequest.Email, resetPasswordRequest.NewPassword);
        var loginRequest = new StringContent(JsonConvert.SerializeObject(login));
        loginRequest.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

        response = await client.PostAsync("/auth/login", loginRequest, CancellationToken.None);
        response.StatusCode.IsSuccess().Should().BeTrue();
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        loginResponse.Should().NotBeNull();
        loginResponse.AccessToken.Should().NotBeNullOrWhiteSpace();
        loginResponse.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }
}
