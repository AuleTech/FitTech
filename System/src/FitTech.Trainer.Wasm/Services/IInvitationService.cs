using AuleTech.Core.Patterns.Result;
using FitTech.ApiClient.Generated;

namespace FitTech.Trainer.Wasm.Services;

public interface IInvitationService
{
    Task<Result<GetInvitationsResponse>> GetTrainerInvitationsAsync(CancellationToken cancellationToken);
    Task<Result> CancelInvitation(string ClientEmail,CancellationToken cancellationToken);
    Task<Result> ResendInvitation(string ClientEmail,CancellationToken cancellationToken);
}
