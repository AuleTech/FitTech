using AwesomeAssertions;
using FitTech.ApiClient;
using FitTech.ApiClient.Generated;
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
        var email = FitTechEmailTestExtensions.GetTestEmail(Guid.NewGuid().ToString()[..4]);
        var password = "TestPassword1234!";
        
        var request = new RegisterTrainerRequest()
        {
          FirstName  = "Test",
          LastName = "Test1",
          Email = email,
          Password = password
        };
        
        var result = await Host.ApiClient.Trainer.RegisterAsync(request, CancellationToken.None);
        result.IsSuccessful.Should().BeTrue();
        
        var loginRequest = new LoginRequest()
        {
            Email = email,
            Password = password
        };

        var loginResult = await Host.ApiClient.Auth.LoginAsync(loginRequest, CancellationToken.None);
        loginResult.Assert();
    }

    [Test]
    public async Task ClientRegistrationFlow_CanInviteAndRetrieveInvitation()
    {
        var testCredentials = await Host.ApiClient.GetTestTrainerCredentialsAsync(CancellationToken.None);
        
        //TODO: Set credentials
        
        var invitationRequest = new InviteClientRequest()
        {
            ClientEmail = FitTechEmailTestExtensions.GetTestEmail(TestContext.Current!.Id.ToString()[..4])
        };
        
        var invitationResult = await Host.ApiClient.Trainer.SendInvitationAsync(invitationRequest, CancellationToken.None);

        invitationResult.IsSuccessful.Should().BeTrue();

        var invitationsResult = await Host.ApiClient.Trainer.GetAllInvitationsAsync(CancellationToken.None);
        invitationsResult.IsSuccessful.Should().BeTrue();

        var invitation = invitationsResult.Content!.Invitations!.Single();
        invitation.ClientEmail.Should().Be(invitationRequest.ClientEmail);
        invitation.Status.Should().Be(nameof(InvitationStatus.Pending));
    }
}
