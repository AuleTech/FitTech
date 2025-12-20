using FitTech.Abstranctions.Dtos;

namespace FitTech.API.Endpoints.Trainer.CancelInvitations;

public record CancelInvitationsResponse(InvitationDto[] Invitations);
