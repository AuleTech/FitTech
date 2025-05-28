using AuleTech.Core.Patterns;
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
    
    Task<Result<NewClientModel>> AddNewClient(string userName, string lastNameUser, DateOnly birthday, int phoneNumber, string subscriptionType, int trainingHours, string trainingType , DateOnly eventTime, string center, CancellationToken cancellationToken);
   
}
