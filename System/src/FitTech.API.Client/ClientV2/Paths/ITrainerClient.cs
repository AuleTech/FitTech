using FitTech.ApiClient.Generated;
using Refit;

namespace FitTech.API.Client.ClientV2.Paths;

public interface ITrainerApiClient
{
    [Post("/trainer/register")]
    Task<IApiResponse> RegisterAsync([Body] RegisterTrainerRequest request, CancellationToken cancellationToken);

    [Get("/trainer/info")]
    Task<IApiResponse<TrainerDataDto>> GetInfoAsync(CancellationToken cancellationToken);

    [Post("/trainer/cancelinvitations")]
    Task<IApiResponse> CancelInvitationAsync([Body] InviteClientRequest request, CancellationToken cancellationToken);

    [Get("/trainer/invitations")]
    Task<IApiResponse<GetInvitationsResponse>> GetAllInvitationsAsync(CancellationToken cancellationToken);
    
    [Post("/trainer/invitations")]
    Task<IApiResponse> SendInvitationAsync([Body] InviteClientRequest request, CancellationToken cancellationToken);

    [Post("/trainer/invitations/resend")]
    Task<IApiResponse> ResendInvitationAsync([Body] InviteClientRequest request, CancellationToken cancellationToken);

    [Post("/trainer/invitations/validate")]
    Task<IApiResponse> ValidateInvitationAsync([Body] ValidateInvitationRequest request, CancellationToken cancellationToken);
}
