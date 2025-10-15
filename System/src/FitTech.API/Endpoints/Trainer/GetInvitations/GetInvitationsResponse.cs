using FitTech.Application.Dtos;

namespace FitTech.API.Endpoints.Trainer.GetInvitations;

public record GetInvitationsResponse(InvitationInfo[] Invitations);

public record InvitationInfo(string ClientEmail, string Status, DateTime CreatedUtc);

public static class GetInvitationEndpointExtensions
{
    public static InvitationInfo FromDto(this InvitationDto dto) => new(dto.ClientEmail, dto.Status, dto.CreatedUtc);
}
