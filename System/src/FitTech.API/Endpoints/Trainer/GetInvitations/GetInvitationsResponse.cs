using FitTech.Abstractions.Dtos;

namespace FitTech.API.Endpoints.Trainer.GetInvitations;

public record GetInvitationsResponse(InvitationDto[] Invitations);
