namespace FitTech.Application.Auth.Dtos;
//TODO: Add Refresh token mechanism
public record LoginDto(string Email, string Password);

public record LoginResultDto(bool Succeeded, string? AccessToken = null)
{
    public static LoginResultDto Failed => new LoginResultDto(false);
}
