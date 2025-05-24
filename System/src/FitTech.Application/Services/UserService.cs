using FitTech.Application.Dtos;
using FitTech.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace FitTech.Application.Services;

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
