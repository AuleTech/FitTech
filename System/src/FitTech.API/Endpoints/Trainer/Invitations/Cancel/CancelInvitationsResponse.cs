using FitTech.Abstractions.Dtos;

namespace FitTech.API.Endpoints.Trainer.Invitations.Cancel;

public record CancelInvitationsResponse(InvitationDto[] Invitations);
