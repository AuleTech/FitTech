namespace FitTech.Application.Auth.Configuration;

public class AuthenticationSettings
{
    public string Audience { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string SigningKey { get; set; } = null!;
    public TimeSpan AccessTokenExpirationTime { get; set; } = TimeSpan.FromMinutes(1);
    public TimeSpan RefreshTokenExpirationTime { get; set; } = TimeSpan.FromHours(24);
}
