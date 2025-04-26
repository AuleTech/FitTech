namespace FitTech.API.Endpoints.Auth.RefreshToken;

public record RefreshTokenRequest(string RefreshToken, string ExpiredAccessToken);
