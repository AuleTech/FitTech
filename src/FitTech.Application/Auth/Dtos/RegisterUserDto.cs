using FitTech.Domain.Entities;

namespace FitTech.Application.Auth.Dtos;

public record RegisterUserDto(string Email, string Password);

public static class RegisterUserDtoMap
{
    internal static FitTechUser MapToIdentityUser(this RegisterUserDto dto) => new()
    {
        Id = Guid.CreateVersion7(),
        UserName = dto.Email,
        Email = dto.Email,
        NormalizedEmail = dto.Email.ToLowerInvariant(),
    };
}
