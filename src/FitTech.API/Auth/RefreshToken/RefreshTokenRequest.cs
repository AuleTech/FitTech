namespace FitTech.API.Auth.RefreshToken;

public record RefreshTokenRequest(string RefreshToken, string ExpiredAccessToken);
