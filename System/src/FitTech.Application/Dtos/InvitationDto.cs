using FitTech.Domain.Aggregates.TrainerAggregate;

namespace FitTech.Application.Dtos;

public record InvitationDto(string ClientEmail, string Status, DateTime CreatedUtc);

internal static class InvitationDtoExtensions
{
    public static InvitationDto ToDto(this Invitation invitation) =>
        new InvitationDto(invitation.Email, invitation.Status.ToString(), invitation.CreatedUtc);
}
