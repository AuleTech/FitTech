namespace FitTech.API.Endpoints.Auth.ResetPassword;

public record ResetPasswordRequest(string Email, string Token, string NewPassword);
