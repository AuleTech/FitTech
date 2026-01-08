using FitTech.Abstractions.Dtos;

namespace FitTech.API.Endpoints.Trainer.ResendInvitations;

public record ResendInvitationsResponse(InvitationDto[] Invitations);
