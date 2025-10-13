using System.Net;
using System.Net.Http.Headers;
using FitTech.API.Client;
using FitTech.ApiClient;
using FitTech.WebComponents.Models;
using FitTech.WebComponents.Persistence;

namespace FitTech.WebComponents.Authentication;

//TODO: Hacky, we need to refactor.
public class FitTechDelegationHandler : DelegatingHandler
{
    private readonly IFitTechApiClient _apiClient;
    private readonly IStorage _storage;

    public FitTechDelegationHandler(IStorage storage, IFitTechApiClientFactory apiClientFactory)
    {
        _storage = storage;
        _apiClient = apiClientFactory.Create();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var user = await _storage.GetItemAsync<FitTechUser>(FitTechUser.StorageKey, cancellationToken);

        if (user is null)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.AccessToken);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            return response;
        }

        return await RefreshTokenAndRetryAsync();

        async Task<HttpResponseMessage> RefreshTokenAndRetryAsync()
        {
            //TODO: Critical section
            try
            {
                var refreshedToken = await _apiClient.RefreshTokenAsync(
                    new RefreshTokenRequest { RefreshToken = user.RefreshToken, ExpiredAccessToken = user.AccessToken },
                    cancellationToken);

                if (!refreshedToken.Succeeded)
                {
                    return response;
                }

                user.AccessToken = refreshedToken.Value;

                await _storage.SetItemAsync(FitTechUser.StorageKey, user, cancellationToken);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.AccessToken);
                return await base.SendAsync(request, cancellationToken);
            }
            catch (Exception)
            {
                return response;
            }
        }
    }
}
