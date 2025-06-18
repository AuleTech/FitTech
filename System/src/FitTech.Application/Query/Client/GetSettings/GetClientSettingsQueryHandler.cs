using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace FitTech.Application.Query.Client.GetSettings;

internal sealed class GetClientSettingsQueryHandler : IQueryHandler<GetClientSettingsQuery, Result<ClientSettingsDto>>
{
    private readonly UserManager<FitTechUser> _userManager;

    public GetClientSettingsQueryHandler(UserManager<FitTechUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<ClientSettingsDto>> HandleAsync(GetClientSettingsQuery query, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(query.Id.ToString()).WaitAsync(cancellationToken);

        if (user is null)
        {
            return Result<ClientSettingsDto>.Failure("User not found");
        }

        return new ClientSettingsDto(user.UserName!, "Trainer name", user.Email!);
    }
}
