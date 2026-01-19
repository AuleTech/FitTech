using AuleTech.Core.Patterns.Result;
using AwesomeAssertions;
using FitTech.ApiClient;
using FitTech.ApiClient.Generated;
using Refit;

namespace FitTech.TestingSupport.Assertions;

public static class FitTechApiClientAssertionExtensions
{
    public static void Assert(this IApiResponse<LoginResponse> response)
    {
        response.IsSuccessful.Should().BeTrue();
        response.Content.Should().NotBeNull();
        response.Content.AccessToken.Should().NotBeNullOrWhiteSpace();
        response.Content.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }
}
