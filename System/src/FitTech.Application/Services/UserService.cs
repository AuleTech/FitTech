using AuleTech.Core.Patterns.Result;
using FitTech.Abstranctions.Dtos;
using FitTech.Domain.Aggregates.AuthAggregate;
using Microsoft.AspNetCore.Identity;

namespace FitTech.Application.Services;

//TODO: Think about this, move to Query or just remove it

public interface IUserService
{
    Task<Result<UserInfoDto>> GetUserInfoAsync(Guid userId, CancellationToken cancellation);
}

internal sealed class UserService : IUserService
{
    private readonly UserManager<FitTechUser> _userManager;

    public UserService(UserManager<FitTechUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<UserInfoDto>> GetUserInfoAsync(Guid userId, CancellationToken cancellation)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString()).WaitAsync(cancellation);

        if (user is null)
        {
            return Result<UserInfoDto>.Failure("UserInfo not found");
        }

        return Result<UserInfoDto>.Success(new UserInfoDto(user.UserName!));
    }
}
