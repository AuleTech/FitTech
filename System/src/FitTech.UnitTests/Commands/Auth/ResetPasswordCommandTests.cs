using AwesomeAssertions;
using FitTech.Application.Commands.Auth.ResetPassword;
using FitTech.Domain.Aggregates.AuthAggregate;
using FitTech.UnitTests.Data.Generators;
using FitTech.UnitTests.Data.Mocks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace FitTech.UnitTests.Commands.Auth;

internal class ResetPasswordCommandTests : BaseCqrsUnitTest<ResetPasswordCommand, ResetPasswordCommandHandler>
{
    private readonly UserManagerMockBuilder _managerMockBuilder = new ();
    protected override ResetPasswordCommandHandler CreateSut() =>
        new (_managerMockBuilder, NullLogger<ResetPasswordCommandHandler>.Instance);
    protected override ResetPasswordCommand CreateRequest() => new (Faker.Internet.Email(),
        Faker.Internet.Password(), Faker.Random.String());
    
    [Test]
    [Arguments("", "", "", 3)]
    [Arguments("email", "", "", 3)]
    [Arguments("email@email.com", "", "", 2)]
    [Arguments("email@email.com", "password", "", 1)]
    [Arguments("wrongemail", "password", "token1234", 1)]
    public async Task ResetPasswordCommand_WhenWrongInput_ReturnsErrors(string email, string password, string token, int expectedErrorCount)
    {
        var sut = CreateSut();
        var result = await sut.HandleAsync(new ResetPasswordCommand(email, password, token), CancellationToken.None);
        result.Errors.Length.Should().Be(expectedErrorCount);
    }

    [Test]
    public async Task ResetPasswordCommand_WhenUserNotFound_ReturnsFailureButNotDetailedException()
    {
        _managerMockBuilder.ConfigureUserStore(x => x.FindByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((FitTechUser?)null));
        
        var sut = CreateSut();
        var result = await sut.HandleAsync(CreateRequest(), CancellationToken.None);
        result.Succeeded.Should().BeFalse();
    }
    
    [Test]
    public async Task ResetPasswordCommand_WhenUserFound_ReturnsOk()
    {
        _managerMockBuilder.ConfigureUserStore(x =>
        {
            x.UpdateAsync(Arg.Any<FitTechUser>(), Arg.Any<CancellationToken>()).Returns(IdentityResult.Success);
            x.FindByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(new FitTechUserTestGenerator());
        });

        var sut = CreateSut();
        var result = await sut.HandleAsync(CreateRequest(), CancellationToken.None);
        result.Succeeded.Should().BeTrue();
    }
}
