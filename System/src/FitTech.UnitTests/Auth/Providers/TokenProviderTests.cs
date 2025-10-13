using System.Security.Claims;
using FitTech.Application.Providers;
using FitTech.UnitTests.Data.Generators;

namespace FitTech.UnitTests.Auth.Providers;

public class TokenProviderTests
{
    private static ITokenProvider? _sut;
    [Before(Class)]
    public static void SetUp()
    {
        var authSettingFaker = new AuthenticationSettingsTestGenerator();
        _sut = new TokenProvider(authSettingFaker);
    }
    
    [Test]
    public async Task GenerateAccessToken_WhenUserIsNotEmpty_GeneratesAJwtToken()
    {
        var fitTechUserFaker = new FitTechUserTestGenerator();
        var result = _sut!.GenerateAccessToken(fitTechUserFaker);

        var tokenClaims = _sut.GetClaimsPrincipalFromAccessToken(result);

        await Assert.That(tokenClaims.Claims).Contains(x => x.Type == ClaimTypes.NameIdentifier);
        await Assert.That(tokenClaims.Claims).Contains(x => x.Type == ClaimTypes.Email);
        await Assert.That(tokenClaims.Claims).Contains(x => x.Type == ClaimTypes.Role);
    }
    
    [Test]
    [Arguments(2)]
    [Arguments(10)]
    [Arguments(100)]
    public async Task GenerateRefreshToken_DoesNotGeneratesTheSame(int times)
    {
        var tokens = new HashSet<string>();

        for (int i = 0; i<times; i++)
        {
            var token = _sut!.GenerateRefreshToken();
            
            if (!tokens.Add(token))
            {
                Assert.Fail($"Duplicated token found: {token}");
            }
        }

        await Assert.That(tokens.Count).IsEqualTo(times);
    }
    
}
