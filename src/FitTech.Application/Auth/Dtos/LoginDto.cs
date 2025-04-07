namespace FitTech.Application.Auth.Dtos;
//TODO: Add Refresh token mechanism
public record LoginDto(string Email, string Password);

public record LoginResultDto(string AccessToken, string RefreshToken);
