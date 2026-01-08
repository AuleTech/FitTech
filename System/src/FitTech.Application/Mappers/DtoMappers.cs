using FitTech.Abstractions.Dtos;
using FitTech.Domain.Aggregates.TrainerAggregate;

namespace FitTech.Application.Mappers;

public static class DtoMappers
{
    public static InvitationDto ToDto(this Invitation invitation)
    {
        return new InvitationDto(invitation.Email, invitation.Status.ToString(), invitation.CreatedUtc);
    }
}
