using System.Net;
using System.Net.Http.Headers;
using Blazored.LocalStorage;
using FitTech.API.Client.Configuration;
using FitTech.Api.Client.Generated;
using FitTech.WebComponents.Models;

namespace FitTech.WebComponents.Authentication;

//TODO: Hacky, we need to refactor.
public class FitTechDelegationHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorageService;
    private readonly FitTechAPIClient _apiClient;

    public FitTechDelegationHandler(ILocalStorageService localStorageService, FitTechApiConfiguration apiConfiguration, IHttpClientFactory httpClientFactory)
    {
        _localStorageService = localStorageService;
        var httpClient = httpClientFactory.CreateClient(nameof(FitTechDelegationHandler));
        httpClient.BaseAddress = new Uri(apiConfiguration.Url);

        _apiClient = new FitTechAPIClient(httpClient);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var user = await _localStorageService.GetItemAsync<FitTechUser>(FitTechUser.StorageKey, cancellationToken);

        if (user is null)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.AccessToken);

        var respone = await base.SendAsync(request, cancellationToken);

        if (respone.StatusCode != HttpStatusCode.Unauthorized)
        {
            return respone;
        }

        return await RefreshTokenAndRetryAsync();

        async Task<HttpResponseMessage> RefreshTokenAndRetryAsync()
        {
            //TODO: Critical section
            var refreshedToken = await _apiClient.FitTechAPIAuthRefreshTokenRefreshTokenEndpointAsync(
                new FitTechAPIAuthRefreshTokenRefreshTokenRequest
                {
                    RefreshToken = user.RefreshToken, ExpiredAccessToken = user.AccessToken
                }, cancellationToken);

            if (!refreshedToken.Succeeded)
            {
                return respone;
            }

            user.AccessToken = refreshedToken.Value;

            await _localStorageService.SetItemAsync(FitTechUser.StorageKey, user, cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.AccessToken);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
