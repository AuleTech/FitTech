using FitTech.Domain.Aggregates.AuthAggregate;
using Microsoft.AspNetCore.Identity;
using NSubstitute;

namespace FitTech.UnitTests.Data.Mocks;

public interface ISuperUserStore<TUser> : IUserEmailStore<TUser>, IUserAuthenticationTokenStore<TUser>, IUserPasswordStore<TUser>
    where TUser : class;

public class UserManagerMockBuilder
{
    private readonly ISuperUserStore<FitTechUser> _emailStore = Substitute.For<ISuperUserStore<FitTechUser>>();
    private readonly IPasswordHasher<FitTechUser> _passwordHasher = Substitute.For<IPasswordHasher<FitTechUser>>();

    private readonly IUserTwoFactorTokenProvider<FitTechUser> _defaultTokenProvider =
        Substitute.For<IUserTwoFactorTokenProvider<FitTechUser>>();
    public UserManagerMockBuilder ConfigureUserStore(Action<ISuperUserStore<FitTechUser>> action)
    {
        action(_emailStore);
        return this;
    }

    public UserManagerMockBuilder ConfigurePasswordHasher(Action<IPasswordHasher<FitTechUser>> action)
    {
        action(_passwordHasher);

        return this;
    }

    public UserManager<FitTechUser> Build()
    {
        var userManager = new UserManager<FitTechUser>(_emailStore, null!, _passwordHasher, null!,
            null!, null!, null!, null!, null!);

        ConfigureDefaultTokenProvider();

        return userManager;

        void ConfigureDefaultTokenProvider()
        {
            _defaultTokenProvider
                .GenerateAsync(Arg.Any<string>(), Arg.Any<UserManager<FitTechUser>>(), Arg.Any<FitTechUser>())
                .Returns("ThisIsaTestToken123456789");
            _defaultTokenProvider.ValidateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<UserManager<FitTechUser>>(), Arg.Any<FitTechUser>()).Returns(true);
            
            userManager.RegisterTokenProvider("Default", _defaultTokenProvider);
        }
    }

    public static implicit operator UserManager<FitTechUser>(UserManagerMockBuilder builder) => builder.Build();
}
