using FitTech.Application.Dtos;

namespace FitTech.Application.Services;

public interface IUserService
{
    Task<Result<UserInfoDto>> GetUserInfoAsync(Guid userId, CancellationToken cancellation);
}
