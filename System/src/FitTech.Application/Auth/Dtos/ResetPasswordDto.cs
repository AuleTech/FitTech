namespace FitTech.Application.Auth.Dtos;

public record ResetPasswordDto(string Email, string Password, string Token);
