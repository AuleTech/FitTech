using FitTech.Abstractions.Dtos;

namespace FitTech.API.Endpoints.Trainer.Invitations.Get;

public record GetInvitationsResponse(InvitationDto[] Invitations);
