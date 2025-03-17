namespace FitTech.API.Auth.ForgotPassword;

public record ForgotPasswordRequest(string Email, string CallbackUrl);
