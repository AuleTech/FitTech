using FitTech.Abstranctions.Dtos;

namespace FitTech.API.Endpoints.Trainer.ResendInvitations;

public record ResendInvitationsResponse(InvitationDto[] Invitations);
