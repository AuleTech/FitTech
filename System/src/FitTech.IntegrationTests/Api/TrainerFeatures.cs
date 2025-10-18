using AwesomeAssertions;
using FitTech.ApiClient;
using FitTech.Domain.Enums;
using FitTech.TestingSupport;
using FitTech.TestingSupport.Assertions;

namespace FitTech.IntegrationTests.Api;

public class TrainerFeatures
{
    [ClassDataSource<TestHost>(Shared = SharedType.PerTestSession)]
    public required TestHost Host { get; init; }

    [Test]
    public async Task RegisterTrainer_HappyPath_RegisterTrainer()
    {
        var client = Host.GetClientApiClient();

        var email = FitTechEmailTestExtensions.GetTestEmail(Guid.NewGuid().ToString()[..4]);
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

    [Test]
    public async Task ClientRegistrationFlow_CanInviteAndRetrieveInvitation()
    {
        var client = Host.GetClientApiClient();
        var testCredentials = await client.GetTestCredentialsAsync(CancellationToken.None);
        
        var authenticatedClient = Host.GetClientApiClient(testCredentials.Token);
        var invitationRequest = new InviteClientRequest()
        {
            ClientEmail = FitTechEmailTestExtensions.GetTestEmail(TestContext.Current!.Id.ToString()[..4])
        };
        
        var invitationResult = await authenticatedClient.SendInvitationAsync(invitationRequest, CancellationToken.None);

        invitationResult.Succeeded.Should().BeTrue(invitationResult.ToString());

        var invitationsResult = await authenticatedClient.GetInvitationsAsync(CancellationToken.None);
        invitationsResult.Succeeded.Should().BeTrue();

        var invitation = invitationsResult.Value!.Invitations!.Single();
        invitation.ClientEmail.Should().Be(invitationRequest.ClientEmail);
        invitation.Status.Should().Be(nameof(InvitationStatus.Pending));
    }
}
