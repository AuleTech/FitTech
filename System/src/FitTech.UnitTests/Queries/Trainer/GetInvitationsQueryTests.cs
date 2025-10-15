using AwesomeAssertions;
using FitTech.Application.Query.Trainer.GetInvitations;
using FitTech.Domain.Enums;
using FitTech.Domain.Repositories;
using FitTech.UnitTests.Data.Generators;
using NSubstitute;

namespace FitTech.UnitTests.Queries.Trainer;

internal class GetInvitationsQueryTests : BaseCqrsUnitTest<GetInvitationsQuery, GetInvitationsQueryHandler>
{
    private readonly ITrainerRepository _trainerRepository = Substitute.For<ITrainerRepository>();
    protected override GetInvitationsQueryHandler CreateSut() => new (_trainerRepository);

    protected override GetInvitationsQuery CreateRequest() => new (Guid.NewGuid());

    [Test]
    public async Task GetInvitationsQuery_WhenTrainerNotFound_ReturnError()
    {
        var sut = CreateSut();
        var result = await sut.HandleAsync(CreateRequest(), CancellationToken.None);
        result.Succeeded.Should().BeFalse();
    }

    [Test]
    public async Task GetInvitationsQuery_WhenExistOldInvitations_ReturnsLast15DaysInvitations()
    {
        var trainer = new TrainerTestGenerator().WithInvitations().Generate();
        _trainerRepository.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(trainer);
        
        var sut = CreateSut();
        var result = await sut.HandleAsync(CreateRequest(), CancellationToken.None);
        result.Succeeded.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var randomInvitation = result.Value.First(x => x.Status != nameof(InvitationStatus.Accepted));
        randomInvitation.Status.Should().BeOneOf(Enum.GetNames<InvitationStatus>());
        randomInvitation.CreatedUtc.Should().NotBeBefore(DateTime.UtcNow.AddDays(-15));
        randomInvitation.ClientEmail.Should().NotBeNullOrWhiteSpace();
    }
}
