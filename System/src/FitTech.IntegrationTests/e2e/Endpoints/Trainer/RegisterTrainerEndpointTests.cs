using System.Net.Http.Headers;
using System.Net.Http.Json;
using AuleTech.Core.System.Http;
using AwesomeAssertions;
using FitTech.API.Endpoints.Auth.Login;
using FitTech.API.Endpoints.Trainer.Register;
using Newtonsoft.Json;

namespace FitTech.IntegrationTests.e2e.Endpoints.Trainer;

public class RegisterTrainerEndpointTests
{
    [ClassDataSource<TestHost>(Shared = SharedType.PerTestSession)]
    public required TestHost Host { get; init; }

    [Test]
    public async Task RegisterTrainer_HappyPath_RegisterTrainer()
    {
        var client = Host.GetClient();

        var email = "test@email.com";
        var password = "TestPassword1234!";
        
        var request = new RegisterTrainerRequest("Test", "Test1", email, password);
        var content = new StringContent(JsonConvert.SerializeObject(request));
        content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
        
        var response = await client.PostAsync("/trainer/register", content, CancellationToken.None);
        response.StatusCode.IsSuccess().Should().BeTrue();
        
        
        var loginRequest = new LoginRequest(email, password);
        var loginRequestContent = new StringContent(JsonConvert.SerializeObject(loginRequest));
        loginRequestContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
        response = await client.PostAsync("/auth/login", loginRequestContent, CancellationToken.None);
        
        response.StatusCode.IsSuccess().Should().BeTrue();
        var loginContent = await response.Content.ReadFromJsonAsync<LoginResponse>();
        
        loginContent.Should().NotBeNull();
        loginContent.AccessToken.Should().NotBeNullOrWhiteSpace();
        loginContent.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }
}
