using AuleTech.Core.Patterns.Result;
using AwesomeAssertions;
using FitTech.ApiClient;

namespace FitTech.IntegrationTests;

internal static class FitTechApiClientAssertionExtensions
{
    public static void Assert(this Result<LoginResponse> response)
    {
        response.Succeeded.Should().BeTrue();
        response.Value.Should().NotBeNull();
        response.Value.AccessToken.Should().NotBeNullOrWhiteSpace();
        response.Value.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }
}
