using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Query.Client.GetSettings;
using FitTech.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace FitTech.Application.Query.Trainer.GetTrainerData;

internal sealed class GetTrainerDataQueryHandler : IQueryHandler<GetTrainerDataQuery, Result<TrainerDataDto>>
{
    private readonly UserManager<FitTechUser> _userManager;

    public GetTrainerDataQueryHandler(UserManager<FitTechUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<TrainerDataDto>> HandleAsync(GetTrainerDataQuery query, CancellationToken cancellationToken)
    {
        var trainer  = await _userManager.FindByIdAsync(query.Id.ToString()).WaitAsync(cancellationToken);

        if (trainer is null)
        {
            return Result<TrainerDataDto>.Failure("Trainer not found");
        }

        //TODO: Forma incorrecta, solamente puesto para que compile. Cuando este completado añadir al ServiceCollectionExtensions
        return new Result<TrainerDataDto>();
    }
}
