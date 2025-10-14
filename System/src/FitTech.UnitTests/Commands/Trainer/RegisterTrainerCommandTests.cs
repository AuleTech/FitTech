using AwesomeAssertions;
using FitTech.Application.Commands.Trainer.Register;
using FitTech.Application.Services;
using FitTech.Domain.Aggregates.AuthAggregate;
using FitTech.Domain.Repositories;
using FitTech.Domain.Seedwork;
using FitTech.UnitTests.Data.Mocks;
using Microsoft.AspNetCore.Identity;
using NSubstitute;

namespace FitTech.UnitTests.Commands.Trainer;

internal class RegisterTrainerCommandTests : BaseCqrsUnitTest<RegisterTrainerCommand, RegisterTrainerCommandHandler>
{
    private readonly ITrainerRepository _trainerRepository = Substitute.For<ITrainerRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IEmailService _emailService = Substitute.For<IEmailService>();
    private readonly UserManagerMockBuilder _managerMockBuilder = new ();

    protected override RegisterTrainerCommandHandler CreateSut() =>
        new (_trainerRepository, _managerMockBuilder, _unitOfWork, _emailService);

    protected override RegisterTrainerCommand CreateRequest() => new (Faker.Person.FirstName,
        Faker.Person.LastName, Faker.Person.Email, Faker.Internet.Password());

    [Test]
    [Arguments("", "", "", "", 4)]
    [Arguments("Paco", "", "", "", 3)]
    [Arguments("Paco", "Zurdo", "", "", 2)]
    [Arguments("Paco", "Zurdo", "paco.zurdo.com", "", 2)]
    [Arguments("Paco", "Zurdo", "paco.zurdo@email.com", "", 1)]
    [Arguments("Paco", "Zurdo", "email.com", "password", 1)]
    public async Task RegisterTrainerCommand_WhenInvalidInput_ReturnValidationErrors(string name, string lastName, string email,string password, int expectedErrorCount)
    {
        var sut = CreateSut();
        var result = await sut.HandleAsync(new RegisterTrainerCommand(name, lastName, email, password),
            CancellationToken.None);
        
        result.Errors.Length.Should().Be(expectedErrorCount);
    }
}
