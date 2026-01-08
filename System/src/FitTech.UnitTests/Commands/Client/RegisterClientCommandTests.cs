using AwesomeAssertions;
using FitTech.Abstractions.Dtos;
using FitTech.Application.Commands.Client.Register;
using FitTech.Domain.Aggregates.AuthAggregate;
using FitTech.Domain.Enums;
using FitTech.Domain.Exceptions;
using FitTech.Domain.Repositories;
using FitTech.Domain.Seedwork;
using FitTech.TestingSupport;
using FitTech.UnitTests.Data.Generators;
using FitTech.UnitTests.Data.Mocks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace FitTech.UnitTests.Commands.Client;

internal class RegisterClientCommandTests : FitTechCqrsUnitTest<RegisterClientCommand, RegisterClientCommandHandler>
{
    private readonly IClientRepository _clientRepository = Substitute.For<IClientRepository>();
    private readonly UserManagerMockBuilder _managerMockBuilder = new();
    private readonly ITrainerRepository _trainerRepository = Substitute.For<ITrainerRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    protected override RegisterClientCommandHandler CreateSut()
    {
        return new RegisterClientCommandHandler(_clientRepository,
            _unitOfWork, _managerMockBuilder, NullLogger<RegisterClientCommandHandler>.Instance, _trainerRepository);
    }

    public RegisterClientCommandTests() : base()
    {
        Request.RuleFor(x => x.InvitationId, _ => Guid.CreateVersion7());
        Request.RuleFor(x => x.Information, _ => FakeGenerators.PersonInfoDto.Value);
        Request.RuleFor(x => x.BodyMeasurement, _ => FakeGenerators.BodyMeasurementDto.Value);
        Request.RuleFor(x => x.Credentials, _ => FakeGenerators.CredentialsDto.Value);
        Request.RuleFor(x => x.TrainingSettings, _ => FakeGenerators.TrainingSettingsDto.Value);
    }

    [Test]
    [Arguments("", "", "", 3)]
    [Arguments("dsad", "", "", 2)]
    [Arguments("", "asd", "", 2)]
    [Arguments("", "", "asds", 2)]
    [Arguments("asdas", "", "asds", 1)]
    public async Task RegisterClientCommand_WhenInvalidPersonInformation_ReturnsFailure(string name, string lastName,
        string phoneNumber, int expectedErrorCount)
    {
        var personInformation =
            new PersonInfoDto(name, lastName, phoneNumber, DateOnly.MaxValue, FakeGenerators.AddressDto.Value);

        Request.RuleForOverride(x => x.Information, _ => personInformation);
        
        var sut = CreateSut();
        var result = await sut.HandleAsync(Request, CancellationToken.None);
        result.Succeeded.Should().BeFalse();
        result.Errors.Length.Should().Be(expectedErrorCount);
    }

    [Test]
    [Arguments("", "", 2)]
    [Arguments("sdas", "", 1)]
    [Arguments("", "asd", 1)]
    public async Task RegisterClientCommand_WhenInvalidCredentials_ReturnsFailure(string email, string password,
        int expectedErrorCount)
    {
        var sut = CreateSut();
        Request.RuleForOverride(x => x.Credentials, _ => new CredentialsDto(email,password));
        
        var result = await sut.HandleAsync(Request, CancellationToken.None);
        result.Succeeded.Should().BeFalse();
        result.Errors.Length.Should().Be(expectedErrorCount);
    }

    [Test]
    public async Task RegisterClientCommand_WhenClientEmailIsDifferentFromInvitation_ReturnsFailure()
    {
        var trainer = new TrainerFakeGenerator().WithInvitations().Generate();
        
        _trainerRepository.GetByInvitationId(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(trainer); 
        
        Request.RuleForOverride(x => x.InvitationId, _ => trainer.Invitations.First().Id);
        
        var sut = CreateSut();
        var result = await sut.HandleAsync(Request, CancellationToken.None);
        
        result.Succeeded.Should().BeFalse();
        result.Errors.Length.Should().Be(1);
        result.Errors.First().Should().Be(TrainerExceptionMessages.InvitationEmailDifferent);
    }
    
    [Test]
    public async Task RegisterClientCommand_WhenClientIsAlreadyInTheTeam_ReturnsFailure()
    {
        var email = FitTechEmailTestExtensions.GetTestEmail();
        var invitation = new InvitationFakeGenerator().WithEmail(email).WithStatus(InvitationStatus.InProgress).Generate();
        var client = new ClientFakeGenerator().WithEmail(email).Generate();

        var trainer = new TrainerFakeGenerator().WithClients([client]).WithInvitations([invitation]).Generate();
        
        _trainerRepository.GetByInvitationId(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(trainer);

        Request.RuleForOverride(x => x.InvitationId, _ => invitation.Id);
        
        var sut = CreateSut();
        var result = await sut.HandleAsync(Request, CancellationToken.None);
        
        result.Succeeded.Should().BeFalse();
        result.Errors.Length.Should().Be(1);
        result.Errors.First().Should().Be(TrainerExceptionMessages.UserAlreadyInTeam);
    }
    
    [Test]
    public async Task RegisterClientCommand_WhenInvitationIsNotInProgress_ReturnsFailure()
    {
        var email = FitTechEmailTestExtensions.GetTestEmail();
        var invitation = new InvitationFakeGenerator().WithEmail(email).WithStatus(InvitationStatus.Completed).Generate();
        var client = new ClientFakeGenerator().WithEmail(email).Generate();

        var trainer = new TrainerFakeGenerator().WithClients([client]).WithInvitations([invitation]).Generate();
        
        _trainerRepository.GetByInvitationId(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(trainer);

        Request.RuleForOverride(x => x.InvitationId, _ => invitation.Id);
        
        var sut = CreateSut();
        var result = await sut.HandleAsync(Request, CancellationToken.None);
        
        result.Succeeded.Should().BeFalse();
        result.Errors.Length.Should().Be(1);
        result.Errors.First().Should().Be(TrainerExceptionMessages.InvitationNoInProgress);
    }

    [Test]
    public async Task RegisterClientCommand_WhenValidInput_ReturnOk()
    {
        var email = FitTechEmailTestExtensions.GetTestEmail();
        var invitation = new InvitationFakeGenerator().WithEmail(email).WithStatus(InvitationStatus.InProgress).Generate();

        var trainer = new TrainerFakeGenerator().WithInvitations([invitation]).Generate();
        
        _trainerRepository.GetByInvitationId(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(trainer);

        _managerMockBuilder.ConfigureUserStore(x =>
            x.CreateAsync(Arg.Any<FitTechUser>(), Arg.Any<CancellationToken>()).Returns(IdentityResult.Success));
        
        Request.RuleForOverride(x => x.InvitationId, _ => invitation.Id);
        
        var sut = CreateSut();
        var result = await sut.HandleAsync(Request, CancellationToken.None);
        result.Succeeded.Should().BeTrue(result.ToString());
    }
}
