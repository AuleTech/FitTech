using FitTech.WebComponents.Models;

namespace FitTech.WebComponents.Services;

public interface IUserService
{
    Task<Result<FitTechUser>> LoginAsync(string email, string password, CancellationToken cancellationToken);
    Task<Result> RegisterAsync(string email, string password, CancellationToken cancellationToken);

    Task<Result> RecoveryAsync(string email, CancellationToken cancellationToken);
}
