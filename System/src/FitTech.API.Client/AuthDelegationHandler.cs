using System.Net;
using System.Net.Http.Headers;
using AuleTech.Core.Extensions.Language;
using FitTech.API.Client.ClientV2.Paths;
using FitTech.ApiClient.Generated;
using Microsoft.Extensions.DependencyInjection;

namespace FitTech.API.Client;

public class AuthDelegationHandler : DelegatingHandler
{
    private readonly ITokenStorage _tokenStorage;
    private readonly IAuthenticationApiClient _authenticationApiClient;

    public AuthDelegationHandler(IServiceProvider serviceProvider)
    {
        _authenticationApiClient = serviceProvider.GetRequiredService<IAuthenticationApiClient>();
        _tokenStorage = serviceProvider.GetRequiredService<ITokenStorage>();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _tokenStorage.GetTokenAsync(cancellationToken);

        if (token.IsEmpty)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.Unauthorized ||
            (response.StatusCode == HttpStatusCode.Unauthorized && token.IsEmpty))
        {
            return response;
        }

        return await RefreshTokenAndRetryAsync();

        async Task<HttpResponseMessage> RefreshTokenAndRetryAsync()
        {
            //TODO: Critical section

            var refreshToken = await _tokenStorage.GetRefreshToken(cancellationToken);

            if (refreshToken.IsEmpty)
            {
                return response;
            }

            var refreshTokenResponse = await _authenticationApiClient.RefreshTokenAsync(
                new RefreshTokenRequest { RefreshToken = refreshToken, ExpiredAccessToken = token },
                cancellationToken);

            if (!refreshTokenResponse.IsSuccessful)
            {
                return response;
            }
            

            await _tokenStorage.SetTokenAsync(refreshTokenResponse.Content!, cancellationToken);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshToken);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
