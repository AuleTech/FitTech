using AwesomeAssertions;
using Bogus;
using FitTech.ApiClient;
using FitTech.ApiClient.Generated;
using FitTech.Domain.Aggregates.TrainerAggregate;
using FitTech.TestingSupport;
using FitTech.TestingSupport.Assertions;
using Microsoft.EntityFrameworkCore;

namespace FitTech.IntegrationTests.Api;

public class ClientFeatures
{
    [ClassDataSource<TestHost>(Shared = SharedType.PerTestSession)]
    public required TestHost Host { get; init; }
    
    [Test]
    public async Task ClientFeatures_RegistrationFlow()
    {
        var client = Host.GetClientApiClient();
        var trainerCredentials = await client.GetTestTrainerCredentialsAsync(CancellationToken.None);

        var authClient = Host.GetClientApiClient(trainerCredentials.Token);

        var invitation = await SendAndGetInvitationAsync();

        var validateInvitation =
            await authClient.ValidateInvitationAsync(invitation.Email, invitation.Code, CancellationToken.None);
        validateInvitation.Succeeded.Should().BeTrue();

        var createClientRequest =
            ApiClientTestExtensions.GenerateRegisterClientTestRequest(invitation.Email, invitation.Id);

        var fitTechClientResult = await authClient.RegisterClientAsync(createClientRequest, CancellationToken.None);
        fitTechClientResult.Succeeded.Should().BeTrue(fitTechClientResult.ToString());

        var loginResult = await client.LoginAsync(new LoginRequest()
        {
            Email = createClientRequest.Credentials!.Email, Password = createClientRequest.Credentials.Password
        }, CancellationToken.None);
        loginResult.Assert();

        async Task<Invitation> SendAndGetInvitationAsync()
        {
            var invitationRequest = new InviteClientRequest() { ClientEmail = FitTechEmailTestExtensions.GetTestEmail() };
            var result = await authClient.SendInvitationAsync(invitationRequest, CancellationToken.None);

            result.Succeeded.Should().BeTrue();

            var dbContext = await Host.GetFitTechApiDbContextAsync(CancellationToken.None);
            var invitation =  await dbContext.Invitations.SingleOrDefaultAsync(x => x.Email == invitationRequest.ClientEmail, CancellationToken.None);
            invitation.Should().NotBeNull();

            return invitation;
        }
    }
}
