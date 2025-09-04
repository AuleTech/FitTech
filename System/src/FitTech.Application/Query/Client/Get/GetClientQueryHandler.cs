
using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Query.Trainer.GetTrainerData;
using FitTech.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace FitTech.Application.Query.Client.Get;

internal sealed class GetClientDataQueryHandler : IQueryHandler<GetClientDataQuery, Result<ClientDataDto>>
{
    private readonly DboContext<Domain.Entities.Client> _userManager;

    public GetClientDataQueryHandler(UserManager<FitTechUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<ClientDataDto>> HandleAsync(GetClientDataQuery query, CancellationToken cancellationToken)
    {
        var client  = await _userManager.FindByIdAsync(query.Id.ToString()).WaitAsync(cancellationToken);

        if (client is null)
        {
            return Result<ClientDataDto>.Failure("Trainer not found");
        }
        
        return new ClientDataDto(trainer.UserName!, trainer.Email!, trainer.PasswordHash!);
    }
}
