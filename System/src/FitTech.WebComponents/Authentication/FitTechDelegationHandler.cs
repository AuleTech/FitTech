using System.Net;
using System.Net.Http.Headers;
using Blazored.LocalStorage;
using FitTech.API.Client;
using FitTech.ApiClient;
using FitTech.WebComponents.Models;
using FitTech.WebComponents.Persistence;

namespace FitTech.WebComponents.Authentication;

//TODO: Hacky, we need to refactor.
public class FitTechDelegationHandler : DelegatingHandler
{
    private readonly IStorage _storage;
    private readonly IFitTechApiClient _apiClient;

    public FitTechDelegationHandler(IStorage storage, IFitTechApiClientFactory apiClientFactory)
    {
        _storage = storage;
        _apiClient = apiClientFactory.Create();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var user = await _storage.GetItemAsync<FitTechUser>(FitTechUser.StorageKey, cancellationToken);

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
            var refreshedToken = await _apiClient.RefreshTokenAsync(
                new RefreshTokenRequest()
                {
                    RefreshToken = user.RefreshToken, ExpiredAccessToken = user.AccessToken
                }, cancellationToken);

            if (!refreshedToken.Succeeded)
            {
                return respone;
            }

            user.AccessToken = refreshedToken.Value;

            await _storage.SetItemAsync(FitTechUser.StorageKey, user, cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.AccessToken);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
