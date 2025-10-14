using AwesomeAssertions;
using FitTech.Application.Commands.Auth.Login;
using FitTech.Application.Providers;
using FitTech.Domain.Aggregates.AuthAggregate;
using FitTech.UnitTests.Data.Generators;
using FitTech.UnitTests.Data.Mocks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace FitTech.UnitTests.Commands;

internal class LoginCommandTests : BaseCqrsUnitTest<LoginCommand, LoginCommandHandler>
{
    private readonly UserManagerMockBuilder _managerMockBuilder = new();
    private readonly ITokenProvider _tokenProvider = Substitute.For<ITokenProvider>();

    protected override LoginCommandHandler CreateSut()
    {
        return new LoginCommandHandler(_managerMockBuilder.Build(),
            NullLogger<LoginCommandHandler>.Instance, _tokenProvider);
    }

    protected override LoginCommand CreateRequest()
    {
        return new LoginCommand(Faker.Internet.Email(), Faker.Internet.Password());
    }

    [Test]
    [Arguments("", "", 2)]
    [Arguments("email", "", 2)]
    [Arguments("email.com", "", 2)]
    [Arguments("pepito@email.com", "", 1)]
    [Arguments("pepito@email.com", "password", 0)]
    public async Task LoginCommand_WhenInvalidCredentials_ReturnErrors(string email, string password,
        int expectedErrorCount)
    {
        var command = new LoginCommand(email, password);
        var sut = CreateSut();

        var result = await sut.HandleAsync(command, CancellationToken.None);
        result.Errors.Length.Should().Be(expectedErrorCount);
    }

    [Test]
    public async Task LoginCommand_WhenEmailNotFound_ReturnsFailedWithoutDetails()
    {
        _managerMockBuilder.ConfigureUserStore(x =>
            x.FindByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((FitTechUser?)null));

        var command = CreateRequest();
        var sut = CreateSut();

        var result = await sut.HandleAsync(command, CancellationToken.None);
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().BeEmpty();
    }

    [Test]
    public async Task LoginCommand_WhenWrongPassword_ReturnsFailedWithoutDetails()
    {
        _managerMockBuilder.ConfigureUserStore(x =>
                x.FindByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                    .Returns(new FitTechUserTestGenerator()))
            .ConfigurePasswordHasher(x =>
                x.VerifyHashedPassword(Arg.Any<FitTechUser>(), Arg.Any<string>(), Arg.Any<string>())
                    .Returns(PasswordVerificationResult.Failed));

        var sut = CreateSut();

        var result = await sut.HandleAsync(CreateRequest(), CancellationToken.None);
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().BeEmpty();
    }

    [Test]
    public async Task LoginCommand_WhenValidCredentials_ReturnsOkAndTokens()
    {
        _tokenProvider.GenerateAccessToken(Arg.Any<FitTechUser>()).Returns("Token123456789");
        _managerMockBuilder.ConfigureUserStore(x =>
                x.FindByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                    .Returns(new FitTechUserTestGenerator()))
            .ConfigurePasswordHasher(x =>
                x.VerifyHashedPassword(Arg.Any<FitTechUser>(), Arg.Any<string>(), Arg.Any<string>())
                    .Returns(PasswordVerificationResult.Success))
            .ConfigureUserStore(x =>
                x.GetTokenAsync(Arg.Any<FitTechUser>(), Arg.Any<string>(), Arg.Any<string>(),
                    Arg.Any<CancellationToken>()).Returns(string.Empty));

        var sut = CreateSut();
        var result = await sut.HandleAsync(CreateRequest(), CancellationToken.None);
        result.Succeeded.Should().BeTrue();
        result.Value!.AccessToken.Should().NotBeNullOrWhiteSpace();
        result.Value.RefreshToken.Should().NotBeNullOrWhiteSpace();

    }
    
    [Test]
    public async Task LoginCommand_WhenValidCredentialsAndValidExistingRefreshToken_ReturnsOkAndTokens()
    {
        _tokenProvider.GenerateAccessToken(Arg.Any<FitTechUser>()).Returns("Token123456789");
        _managerMockBuilder.ConfigureUserStore(x =>
                x.FindByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                    .Returns(new FitTechUserTestGenerator()))
            .ConfigurePasswordHasher(x =>
                x.VerifyHashedPassword(Arg.Any<FitTechUser>(), Arg.Any<string>(), Arg.Any<string>())
                    .Returns(PasswordVerificationResult.Success))
            .ConfigureUserStore(x =>
                x.GetTokenAsync(Arg.Any<FitTechUser>(), Arg.Any<string>(), Arg.Any<string>(),
                    Arg.Any<CancellationToken>()).Returns("RefreshToken123456789"));

        var sut = CreateSut();
        var result = await sut.HandleAsync(CreateRequest(), CancellationToken.None);
        result.Succeeded.Should().BeTrue();
        result.Value!.AccessToken.Should().NotBeNullOrWhiteSpace();
        result.Value.RefreshToken.Should().NotBeNullOrWhiteSpace();

    }
}
