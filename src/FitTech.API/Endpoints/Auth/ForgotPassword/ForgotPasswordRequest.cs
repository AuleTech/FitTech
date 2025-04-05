namespace FitTech.API.Endpoints.Auth.ForgotPassword;

public record ForgotPasswordRequest(string Email, string CallbackUrl);
