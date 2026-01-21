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
        var trainerCredentials = await Host.ApiClient.GetTestTrainerCredentialsAsync(CancellationToken.None);

        var invitation = await SendAndGetInvitationAsync();

        var validateInvitation =
            await Host.ApiClient.Trainer.ValidateInvitationAsync(invitation.Email, invitation.Code.ToString(), CancellationToken.None);
        validateInvitation.IsSuccessful.Should().BeTrue();

        var createClientRequest =
            ApiClientTestExtensions.GenerateRegisterClientTestRequest(invitation.Email, invitation.Id);

        var fitTechClientResult = await Host.ApiClient.Client.RegisterAsync(createClientRequest, CancellationToken.None);
        fitTechClientResult.IsSuccessful.Should().BeTrue();

        var loginResult = await Host.ApiClient.Auth.LoginAsync(new LoginRequest()
        {
            Email = createClientRequest.Credentials!.Email, Password = createClientRequest.Credentials.Password
        }, CancellationToken.None);
        loginResult.Assert();

        async Task<Invitation> SendAndGetInvitationAsync()
        {
            var invitationRequest = new InviteClientRequest() { ClientEmail = FitTechEmailTestExtensions.GetTestEmail() };
            var result = await Host.ApiClient.Trainer.SendInvitationAsync(invitationRequest, CancellationToken.None);

            result.IsSuccessful.Should().BeTrue();

            var dbContext = await Host.GetFitTechApiDbContextAsync(CancellationToken.None);
            var invitation =  await dbContext.Invitations.SingleOrDefaultAsync(x => x.Email == invitationRequest.ClientEmail, CancellationToken.None);
            invitation.Should().NotBeNull();

            return invitation;
        }
    }
}
