using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Domain.Aggregates.AuthAggregate;
using Microsoft.AspNetCore.Identity;

namespace FitTech.Application.Query.Trainer.GetTrainerData;

internal sealed class GetTrainerDataQueryHandler : IQueryHandler<GetTrainerDataQuery, Result<TrainerDataDto>>
{
    private readonly UserManager<FitTechUser> _userManager;

    public GetTrainerDataQueryHandler(UserManager<FitTechUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<TrainerDataDto>> HandleAsync(GetTrainerDataQuery query,
        CancellationToken cancellationToken)
    {
        var trainer = await _userManager.FindByIdAsync(query.Id.ToString()).WaitAsync(cancellationToken);

        if (trainer is null)
        {
            return Result<TrainerDataDto>.Failure("Trainer not found");
        }

        return new TrainerDataDto(trainer.UserName!, trainer.Email!, trainer.PasswordHash!);
    }
}
