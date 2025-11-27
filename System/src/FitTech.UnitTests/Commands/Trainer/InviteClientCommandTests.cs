using AwesomeAssertions;
using FitTech.Application.Commands.Trainer.InviteClient;
using FitTech.Application.Services;
using FitTech.Domain.Repositories;
using FitTech.Domain.Seedwork;
using FitTech.UnitTests.Data.Generators;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace FitTech.UnitTests.Commands.Trainer;

internal static class InviteClientCommandTestsDataSource
{
    public static IEnumerable<Func<(Guid, string, int)>> ValidationDataSource()
    {
        yield return () => (Guid.Empty, "", 2);
        yield return () => (Guid.NewGuid(), "", 1);
        yield return () => (Guid.Empty, "email@email.com", 1);
        yield return () => (Guid.NewGuid(), "email@", 1);
    }  
}

internal class InviteClientCommandTests : BaseCqrsUnitTest<InviteClientCommand, InviteClientCommandHandler>
{
    private readonly IEmailService _emailService = Substitute.For<IEmailService>();
    private readonly ITrainerRepository _trainerRepository = Substitute.For<ITrainerRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    protected override InviteClientCommandHandler CreateSut()
    {
        return new InviteClientCommandHandler(_trainerRepository, _unitOfWork, _emailService,
            NullLogger<InviteClientCommandHandler>.Instance);
    }

    protected override InviteClientCommand CreateRequest() =>
        new (Guid.NewGuid(), Faker.Person.Email);

    [Test]
    [MethodDataSource(typeof(InviteClientCommandTestsDataSource), nameof(InviteClientCommandTestsDataSource.ValidationDataSource))]
    public async Task InviteClientCommand_WhenValidationFails_ReturnsErrors(Guid trainerId, string clientEmail, int expectedErrorsCount)
    {
        var sut = CreateSut();
        var result = await sut.HandleAsync(new InviteClientCommand(trainerId, clientEmail), CancellationToken.None);
        result.Errors.Length.Should().Be(expectedErrorsCount);
    }

    [Test]
    public async Task InviteClientCommand_WhenTrainerNotFound_ThrowsUnauthorizedException()
    {
        var sut = CreateSut();
        var act = async () => await sut.HandleAsync(CreateRequest(), CancellationToken.None);
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test]
    public async Task InviteClientCommand_WhenHappyPath_ReturnsOk()
    {
        var trainer = new TrainerFakeGenerator().Generate();
        _trainerRepository.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(trainer);
        var sut = CreateSut();
        var result = await sut.HandleAsync(CreateRequest(), CancellationToken.None);
        result.Succeeded.Should().BeTrue();
        trainer.Invitations.Count.Should().Be(1);
    }
}
