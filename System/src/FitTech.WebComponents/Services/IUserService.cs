using System.Runtime.InteropServices.JavaScript;
using AuleTech.Core.Patterns;
using AuleTech.Core.Patterns.Result;
using Blazor.Heroicons.Solid;
using FitTech.ApiClient;
using FitTech.WebComponents.Models;
using FitTech.WebComponents.Pages.CustomerManager.CustomerManagerModels;

namespace FitTech.WebComponents.Services;

public interface IUserService
{
    Task<bool> IsLoggedAsync();
    Task<Result<FitTechUser>> LoginAsync(string email, string password, CancellationToken cancellationToken);
    Task<Result> RegisterAsync(string email, string password, CancellationToken cancellationToken);
    Task<Result<string>> ForgotPasswordAsync(string email, CancellationToken cancellationToken);
    Task<Result> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken);
    Task<Result> LogoutAsync(CancellationToken cancellationToken);
    Task<Result> AddNewClientAsync(string username, string lastname, DateTime birthdate, string email, int? phoneNumber, int? trainingHours, string trainingMode, string center, DateTime eventDate, string subscriptionType,  CancellationToken cancellationToken);
    Task<Result<TrainerDataDto>> GetTrainerDataAsync(CancellationToken cancellationToken);
    
}
