using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Dtos;
using FitTech.Application.Extensions;
using FitTech.Domain.Enums;
using FitTech.Domain.Repositories;

namespace FitTech.Application.Query.Trainer.GetInvitations;

public interface IGetInvitationQueryHandler : IQueryHandler<GetInvitationsQuery, Result<InvitationDto[]>>;

internal class GetInvitationsQueryHandler : IGetInvitationQueryHandler
{
    private readonly ITrainerRepository _trainerRepository;

    public GetInvitationsQueryHandler(ITrainerRepository trainerRepository)
    {
        _trainerRepository = trainerRepository;
    }

    public async Task<Result<InvitationDto[]>> HandleAsync(GetInvitationsQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = query.Validate();

        if (!validationResult.Succeeded)
        {
            return validationResult.ToTypedResult<InvitationDto[]>();
        }

        var trainer = await _trainerRepository.GetAsync(query.TrainerId, cancellationToken);

        if (trainer is null)
        {
            return Result<InvitationDto[]>.Failure("Trainer not found");
        }

        return trainer!.Invitations.Where(x =>
                !(x.Status == InvitationStatus.Accepted && x.CreatedUtc < DateTime.UtcNow.AddDays(-15)))
            .Select(x => x.ToDto())
            .ToArray();
    }
}
