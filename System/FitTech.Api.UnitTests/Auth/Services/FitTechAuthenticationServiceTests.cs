using FitTech.Application.Auth.Dtos;
using FitTech.Application.Auth.Providers;
using FitTech.Application.Auth.Services;
using FitTech.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace FitTech.Api.UnitTests.Auth.Services;

public class FitTechAuthenticationServiceTests
{
    //SUT -> Service under Test
    private readonly ILogger<FitTechAuthenticationService> _logger =
        Substitute.For<ILogger<FitTechAuthenticationService>>();
    private readonly ITokenProvider _tokenProvider = Substitute.For<ITokenProvider>();
    private readonly UserManager<FitTechUser> _userManager = CreateMockUserManager<FitTechUser>();

    private FitTechAuthenticationService BuildSut() =>
        new FitTechAuthenticationService(_userManager, _logger, _tokenProvider);

    [Test]
    [Timeout(10000)]
    public async Task RegisterAsync_WhenRegisterUserDtoIsProvided_ThenResultSuccess(CancellationToken cancellationToken)
    {
       _userManager
            .CreateAsync(Arg.Any<FitTechUser>(), Arg.Any<string>())
            .Returns(IdentityResult.Success);
        
        var sut = BuildSut();
        var result = await sut.RegisterAsync(new RegisterUserDto("", ""), cancellationToken);

        await Assert.That(result.Succeeded).IsTrue();
    }
    
    private static UserManager<TUser> CreateMockUserManager<TUser>() where TUser : class
    {
        var store = Substitute.For<IUserStore<TUser>>();
        var options = Substitute.For<IOptions<IdentityOptions>>();
        var passwordHasher = Substitute.For<IPasswordHasher<TUser>>();
        var userValidators = new List<IUserValidator<TUser>>();
        var passwordValidators = new List<IPasswordValidator<TUser>>();
        var keyNormalizer = Substitute.For<ILookupNormalizer>();
        var errors = Substitute.For<IdentityErrorDescriber>();
        var services = Substitute.For<IServiceProvider>();
        var logger = Substitute.For<ILogger<UserManager<TUser>>>();

        return Substitute.For<UserManager<TUser>>(
            store,
            options,
            passwordHasher,
            userValidators,
            passwordValidators,
            keyNormalizer,
            errors,
            services,
            logger
        );
    }
} 
