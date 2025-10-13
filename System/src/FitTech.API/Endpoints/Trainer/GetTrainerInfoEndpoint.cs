using System.Security.Claims;
using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FastEndpoints;
using FitTech.Application.Query.Trainer.GetTrainerData;

namespace FitTech.API.Endpoints.Trainer;

[HttpGet("/Trainer/TrainerSettings")]
public class GetTrainerSettingsEndpoint : EndpointWithoutRequest<TrainerDataDto>
{
    private readonly IQueryHandler<GetTrainerDataQuery, Result<TrainerDataDto>> _queryHandler;

    public GetTrainerSettingsEndpoint(IQueryHandler<GetTrainerDataQuery, Result<TrainerDataDto>> queryHandler)
    {
        _queryHandler = queryHandler;
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var TrainerSettings = await _queryHandler.HandleAsync(new GetTrainerDataQuery(Guid.Parse(userId)), ct);

        if (!TrainerSettings.Succeeded)
        {
            ThrowError(TrainerSettings.Errors.First());
        }

        await Send.OkAsync(TrainerSettings.Value!, ct);
    }
}
