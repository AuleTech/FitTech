using System.Runtime.InteropServices.ComTypes;
using Bogus;
using FitTech.API.Client;
using FitTech.ApiClient;
using FitTech.TestingSupport.Models;

namespace FitTech.TestingSupport;

public static class ApiClientTestExtensions
{
    private static readonly Faker Faker = new Faker();
    
    public static async Task<TestCredentials> GetTestCredentialsAsync(this IFitTechApiClient apiClient, CancellationToken cancellationToken)
    {
        var request = new RegisterTrainerRequest()
        {
            FirstName  = Faker.Person.FirstName,
            LastName = Faker.Person.LastName,
            Email = FitTechEmailTestExtensions.GetTestEmail(Guid.NewGuid().ToString()[..4]),
            Password = $"{Faker.Internet.Password()}1!"
        };
        
        var result = await apiClient.RegisterTrainerAsync(request, CancellationToken.None);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(result.ToString());
        }
        
        var loginRequest = new LoginRequest()
        {
            Email = request.Email,
            Password = request.Password
        };

        var loginResult = await apiClient.LoginAsync(loginRequest, CancellationToken.None);

        return new TestCredentials(request.Email, request.Password, loginResult.Value!.AccessToken!);
    }
}
