using FitTech.Abstractions.Dtos;

namespace FitTech.API.Endpoints.Trainer.Invitations.Resend;

public record ResendInvitationsResponse(InvitationDto[] Invitations);
