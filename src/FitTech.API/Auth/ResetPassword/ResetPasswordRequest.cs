namespace FitTech.API.Auth.ResetPassword;

public record ResetPasswordRequest(string Email, string Token, string NewPassword);
