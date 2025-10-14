using AwesomeAssertions;
using Bogus;
using FitTech.Application.Commands.Auth.ForgotPassword;
using FitTech.Application.Services;
using FitTech.Domain.Aggregates.AuthAggregate;
using FitTech.UnitTests.Data.Generators;
using FitTech.UnitTests.Data.Mocks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace FitTech.UnitTests.Commands;

public class ForgotPasswordCommandTests
{
    private readonly IEmailService _emailService = Substitute.For<IEmailService>();
    private readonly ILogger<ForgotPasswordCommandHandler> _logger = Substitute.For<ILogger<ForgotPasswordCommandHandler>>();
    private readonly UserManagerMockBuilder _managerMockBuilder = new UserManagerMockBuilder();
    private ForgotPasswordCommandHandler CreateSut() => new (_logger, _managerMockBuilder.Build(), _emailService);
    
    [Test]
    [Arguments("","",2)]
    [Arguments("email", "",2)]
    [Arguments("email@email.com", "",1)]
    [Arguments("email@email.com", "random",1)]
    [Arguments("email", "callback.com",1)]
    [Arguments("email", "www.thisisatest.com",1)]
    public async Task ForgotPasswordCommand_WhenInvalidFields_ReturnsValidationErrors(string email, string callbackUrl, int expectedErrors)
    {
        var sut = CreateSut();

        var result = await sut.HandleAsync(new ForgotPasswordCommand(email, callbackUrl), CancellationToken.None);
        result.Errors.Length.Should().Be(expectedErrors);
    }

    [Test]
    public async Task ForgotPasswordCommand_WhenUserEmailDoesNotExists_ReturnsOk()
    {
        _managerMockBuilder.ConfigureUserMailStore((userStore) =>
            userStore.FindByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((FitTechUser?)null));

        var sut = CreateSut();

        var result = await sut.HandleAsync(CreateCommand(), CancellationToken.None);
        result.Succeeded.Should().BeTrue();
        result.Value.Should().BeNullOrWhiteSpace();
    }

    [Test]
    public async Task ForgotPasswordCommand_WhenEmailExists_ReturnsToken()
    {
        _managerMockBuilder.ConfigureUserMailStore(x =>
            x.FindByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(new FitTechUserTestGenerator()));

        var sut = CreateSut();

        var result = await sut.HandleAsync(CreateCommand(), CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        result.Value.Should().NotBeNullOrWhiteSpace();
    }

    private ForgotPasswordCommand CreateCommand()
    {
        var faker = new Faker();
        
        return new ForgotPasswordCommand(faker.Internet.Email(), faker.Internet.Url());
    }
}
