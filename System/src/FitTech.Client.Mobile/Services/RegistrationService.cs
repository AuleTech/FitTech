using AuleTech.Core.Patterns.Result;
using FitTech.API.Client;
using FitTech.ApiClient.Generated;
using FitTech.Client.Mobile.Pages.Auth.Registration.Models;

namespace FitTech.Client.Mobile.Services;

internal interface IRegistrationService
{
    Task<Result> RegisterAsync(RegistrationForm registrationForm, CancellationToken cancellationToken);
} 

internal class RegistrationService : IRegistrationService
{
    private readonly IFitTechApiClient _apiClient;

    public RegistrationService(IFitTechApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<Result> RegisterAsync(RegistrationForm registrationForm, CancellationToken cancellationToken)
    {
        var request = registrationForm.ToRegisterClientRequest();

        return await _apiClient.RegisterClientAsync(request, cancellationToken);
    }
}
