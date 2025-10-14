using AwesomeAssertions;
using FitTech.TestingSupport;

namespace FitTech.IntegrationTests.Api;

public class TrainerFeatures
{
    [ClassDataSource<TestHost>(Shared = SharedType.PerTestSession)]
    public required TestHost Host { get; init; }

    [Test]
    public async Task RegisterTrainer_HappyPath_RegisterTrainer()
    {
        var client = Host.GetClientApiClient();

        var email = $"{TestContext.Current!.Id.ToString()[..4]}test@email.com";
        var password = "TestPassword1234!";
        
        var request = new ApiClient.RegisterTrainerRequest()
        {
          FirstName  = "Test",
          LastName = "Test1",
          Email = email,
          Password = password
        };
        
        var result = await client.RegisterTrainerAsync(request, CancellationToken.None);
        result.Succeeded.Should().BeTrue();
        
        var loginRequest = new ApiClient.LoginRequest()
        {
            Email = email,
            Password = password
        };

        var loginResult = await client.LoginAsync(loginRequest, CancellationToken.None);
        loginResult.Assert();
    }
}
