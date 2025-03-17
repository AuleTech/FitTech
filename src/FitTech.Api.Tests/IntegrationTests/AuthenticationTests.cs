using Bogus;
using FitTech.Api.Tests.Models;
using FitTech.Application;
using FitTech.Application.Auth.Dtos;
using FitTech.Application.Auth.Services;
using FitTech.Domain.Entities;
using FitTech.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TUnit.Core.Extensions;

namespace FitTech.Api.Tests.IntegrationTests;

public class AuthenticationTests
{
    private static IFitTechAuthenticationService? _sut;
    private static IServiceProvider? _serviceProvider;
    private readonly Faker _faker = new ();
    
    [Before(Class)]
    public static Task SetUp()
    {
        var serviceCollection = new ServiceCollection();
        var configurationBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection([
                new KeyValuePair<string, string?>("Authentication:Audience", "testAudience"),
                new KeyValuePair<string, string?>("Authentication:Issuer", "testIssuer"),
                new KeyValuePair<string, string?>("Authentication:SigningKey", "ThisIsALocalKey1234ThisIsALocalKey1234!")
            ]);
        
        serviceCollection.AddAuth(configurationBuilder.Build()).AddLogging();
        
        serviceCollection.AddIdentity<FitTechUser, FitTechRole>(options =>
        {
            options.Password.RequiredLength = 8;
        }).AddEntityFrameworkStores<FitTechDbContext>();
        
        serviceCollection.AddInMemorydb(Guid.CreateVersion7().ToString());
        
        var serviceProvider = serviceCollection.BuildServiceProvider();

        _serviceProvider = serviceProvider;
        _sut = serviceProvider.GetRequiredService<IFitTechAuthenticationService>();
        return Task.CompletedTask;
    }

    [Test]
    [Timeout(30_000)] //TODO: Is it possible to: debug ? 1h : 10s
    public async Task RegisterAsync_WhenEmailAndPasswordAreOk_ReturnsSucceeded(CancellationToken cancellationToken)
    {
        var registerDto = new RegisterUserDto(_faker.Person.Email,
            "TestPassword123?"); //TODO: https://github.com/bchavez/Bogus/issues/219 use for create custom passwords
        
        var result =
            await _sut!.RegisterAsync(
                registerDto, cancellationToken);

        await Assert.That(result.Succeeded).IsTrue();
        await Assert.That(result.Errors).IsEmpty();
        
        TestContext.Current!.ObjectBag.Add(TestUserInfo.SharedKey, new TestUserInfo
        {
            Email = registerDto.Email,
            Password = registerDto.Password
        });
    }

    [Test]
    [Timeout(30_000)]
    [DependsOn(nameof(RegisterAsync_WhenEmailAndPasswordAreOk_ReturnsSucceeded))]
    public async Task LoginAsync_WhenEmailAndPasswordAreOk_ReturnAccessToken(CancellationToken cancellationToken)
    {
        var registerAsyncTestContext = TestContext.Current!.GetTests(nameof(RegisterAsync_WhenEmailAndPasswordAreOk_ReturnsSucceeded))
            .First();

        var userInfo = registerAsyncTestContext.ObjectBag[TestUserInfo.SharedKey] as TestUserInfo;

        await Assert.That(userInfo).IsNotNull();

        var loginResult = await _sut!.LoginAsync(new LoginDto(userInfo!.Email, userInfo.Password), cancellationToken);

        await Assert.That(loginResult.Succeeded).IsTrue();
        await Assert.That(loginResult.Value!.AccessToken).IsNotNullOrWhitespace();
    }

    // [Test]
    // [Timeout(30_000)]
    // [DependsOn(nameof(RegisterAsync_WhenEmailAndPasswordAreOk_ReturnsSucceeded))]
    // public async Task ForgotPasswordAsync_WhenEmailIsOk_GeneratesResetPasswordToken(
    //     CancellationToken cancellationToken)
    // {
    //     var registerTestContext = TestContext.Current!.GetTests(nameof(RegisterAsync_WhenEmailAndPasswordAreOk_ReturnsSucceeded))
    //         .First();
    //
    //     var userInfo = registerTestContext.ObjectBag[TestUserInfo.SharedKey] as TestUserInfo;
    //
    //     var result =
    //         await _sut!.ForgotPasswordAsync(new ForgotPasswordDto(userInfo!.Email, "nevermindurl"), cancellationToken);
    //     var db = _serviceProvider!.GetRequiredService<FitTechDbContext>();
    //     var token = db.Set<>()
    // }
}
