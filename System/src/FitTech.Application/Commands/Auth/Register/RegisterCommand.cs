using AuleTech.Core.Patterns.CQRS;
using FitTech.Domain.Entities;

namespace FitTech.Application.Commands.Auth.Register;

public record RegisterCommand(string Email, string Password) : ICommand;

public static class RegisterCommandMap
{
    internal static FitTechUser MapToIdentityUser(this RegisterCommand dto)
    {
        return new FitTechUser
        {
            Id = Guid.CreateVersion7(),
            UserName = dto.Email,
            Email = dto.Email,
            NormalizedEmail = dto.Email.ToLowerInvariant()
        };
    }
}
