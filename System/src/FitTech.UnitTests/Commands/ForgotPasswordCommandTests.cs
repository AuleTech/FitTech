using AwesomeAssertions;
using Bogus;
using FitTech.Application.Commands.Auth.ForgotPassword;
using FitTech.Application.Services;
using FitTech.Domain.Aggregates.AuthAggregate;
using FitTech.UnitTests.Data.Generators;
using FitTech.UnitTests.Data.Mocks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace FitTech.UnitTests.Commands;

internal class ForgotPasswordCommandTests : BaseCqrsUnitTest<ForgotPasswordCommand, ForgotPasswordCommandHandler>
{
    private readonly IEmailService _emailService = Substitute.For<IEmailService>();
    private readonly UserManagerMockBuilder _managerMockBuilder = new UserManagerMockBuilder();
    protected override ForgotPasswordCommandHandler CreateSut() => new (NullLogger<ForgotPasswordCommandHandler>.Instance, _managerMockBuilder.Build(), _emailService);
    protected override ForgotPasswordCommand CreateRequest() => new (Faker.Internet.Email(), Faker.Internet.Url());

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
        _managerMockBuilder.ConfigureUserStore((userStore) =>
            userStore.FindByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((FitTechUser?)null));

        var sut = CreateSut();

        var result = await sut.HandleAsync(CreateRequest(), CancellationToken.None);
        result.Succeeded.Should().BeTrue();
        result.Value.Should().BeNullOrWhiteSpace();
    }

    [Test]
    public async Task ForgotPasswordCommand_WhenEmailExists_ReturnsToken()
    {
        _managerMockBuilder.ConfigureUserStore(x =>
            x.FindByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(new FitTechUserTestGenerator()));

        var sut = CreateSut();

        var result = await sut.HandleAsync(CreateRequest(), CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        result.Value.Should().NotBeNullOrWhiteSpace();
    }
}
