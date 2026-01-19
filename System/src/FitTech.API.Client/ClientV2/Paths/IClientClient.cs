using FitTech.ApiClient.Generated;
using Refit;

namespace FitTech.API.Client.ClientV2.Paths;

public interface IClientApiClient
{
    [Post("/client/register")]
    Task<IApiResponse> RegisterAsync([Body] RegisterClientRequest request, CancellationToken cancellationToken);
}
