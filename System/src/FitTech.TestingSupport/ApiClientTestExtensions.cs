using System.Runtime.InteropServices.ComTypes;
using Bogus;
using FitTech.API.Client;
using FitTech.API.Client.Client;
using FitTech.ApiClient;
using FitTech.ApiClient.Generated;
using FitTech.TestingSupport.Models;

namespace FitTech.TestingSupport;

public static class ApiClientTestExtensions
{
    private static readonly Bogus.Faker Faker = new Bogus.Faker();
    
    public static async Task<TestCredentials> GetTestTrainerCredentialsAsync(this IFitTechApiClient apiClient, CancellationToken cancellationToken)
    {
        var request = new RegisterTrainerRequest()
        {
            FirstName  = Faker.Person.FirstName,
            LastName = Faker.Person.LastName,
            Email = FitTechEmailTestExtensions.GetTestEmail(Guid.NewGuid().ToString()[..4]),
            Password = $"{Faker.Internet.Password()}A1!"
        };
        
        var result = await apiClient.Trainer.RegisterAsync(request, cancellationToken);

        if (!result.IsSuccessful)
        {
            throw new InvalidOperationException(result.ToString());
        }
        
        var loginRequest = new LoginRequest()
        {
            Email = request.Email,
            Password = request.Password
        };

        var loginResult = await apiClient.Auth.LoginAsync(loginRequest, cancellationToken);

        return new TestCredentials(request.Email, request.Password, loginResult.Content!.AccessToken!);
    }

    public static RegisterClientRequest GenerateRegisterClientTestRequest(string email, Guid invitationId) => new ()
    {
        InvitationId = invitationId,
        Information =
            new PersonInfoDto
            {
                Name = Faker.Person.FirstName,
                LastName = Faker.Person.LastName,
                PhoneNumber = Faker.Person.Phone,
                BirthDate = Faker.Date.Between(DateTime.UtcNow.AddYears(-17), DateTime.UtcNow.AddYears(-50)),
                Address = new AddressDto { City = Faker.Address.City(), Country = Faker.Address.Country() }
            },
        Credentials = new CredentialsDto { Email = email, Password = $"{Faker.Internet.Password()}A1!" },
        TrainingSettings =
            new TrainingSettingsDto
            {
                TotalDaysPerWeek = 4, Goal = TrainingGoalEnumDto.Recovery, FavouriteExercises = []
            },
        BodyMeasurement = new BodyMeasurementDto
        {
            Hip = Faker.Random.Number(10, 50),
            MaxThigh = Faker.Random.Number(10, 50),
            Biceps = Faker.Random.Number(10, 50),
            XShoulders = Faker.Random.Number(10, 50),
            Chest = Faker.Random.Number(10, 50),
            Height = Faker.Random.Number(10, 50),
            Weight = Faker.Random.Number(10, 50)
        }
    };
}
