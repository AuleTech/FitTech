using AuleTech.Core.Patterns.Result;
using FitTech.API.Client; 
using FitTech.ApiClient.Generated;
using Result = AuleTech.Core.Patterns.Result.Result;

namespace FitTech.Trainer.Wasm.Services;

internal sealed class InvitationService : IInvitationService
{
    
    private readonly IFitTechApiClient _fitTechApiClient;
    
    public InvitationService(IFitTechApiClient fitTechApiClient)
    {
        _fitTechApiClient = fitTechApiClient;
    }
    
    public async Task<Result<GetInvitationsResponse>> GetTrainerInvitationsAsync(CancellationToken cancellationToken)
    {
        var result = await _fitTechApiClient.GetInvitationsAsync(cancellationToken);

        if (!result.Succeeded)
        {
            return result.MapFailure<GetInvitationsResponse>();
        }
        
        return result;
    }

    public async Task<Result> CancelInvitation(string clientEmail, CancellationToken cancellationToken)
    {
        await _fitTechApiClient.CancelInvitationsAsync(new InviteClientRequest{ClientEmail = clientEmail}, cancellationToken);
        
        return Result.Success;
    }

    public async Task<Result> ResendInvitation(string clientEmail, CancellationToken cancellationToken)
    {
        await _fitTechApiClient.ResendInvitationsAsync(new InviteClientRequest{ClientEmail = clientEmail}, cancellationToken);
        
        return Result.Success;
    }
}
