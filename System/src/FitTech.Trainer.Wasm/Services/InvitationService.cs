using AuleTech.Core.Patterns.Result;
using FitTech.API.Client;
using FitTech.API.Client.ClientV2;
using FitTech.ApiClient.Generated;
using FitTech.Trainer.Wasm.Constants;
using Result = AuleTech.Core.Patterns.Result.Result;

namespace FitTech.Trainer.Wasm.Services;

internal sealed class InvitationService : IInvitationService
{
    private readonly IFitTechApiClientV2 _apiClient;
    
    public InvitationService(IFitTechApiClientV2 apiClient)
    {
        _apiClient = apiClient;
    }
    
    public async Task<Result<GetInvitationsResponse>> GetTrainerInvitationsAsync(CancellationToken cancellationToken)
    {
        var result = await _apiClient.Trainer.GetAllInvitationsAsync(cancellationToken);

        if (!result.IsSuccessful)
        {
            return Result<GetInvitationsResponse>.Failure(result.Error.Content ?? ErrorConstants.GenericError);
        }
        
        return result.Content;
    }

    public async Task<Result> CancelInvitation(string clientEmail, CancellationToken cancellationToken)
    {
        var response = await _apiClient.Trainer.CancelInvitationAsync(new InviteClientRequest{ClientEmail = clientEmail}, cancellationToken);
        
        return response.IsSuccessful ? Result.Success : Result.Failure(response.Error?.Content ?? ErrorConstants.GenericError);
    }

    public async Task<Result> ResendInvitation(string clientEmail, CancellationToken cancellationToken)
    {
        var response = await _apiClient.Trainer.ResendInvitationAsync(new InviteClientRequest{ClientEmail = clientEmail}, cancellationToken);
        
        return response.IsSuccessful ? Result.Success : Result.Failure(response.Error?.Content ?? ErrorConstants.GenericError);
    }
}
