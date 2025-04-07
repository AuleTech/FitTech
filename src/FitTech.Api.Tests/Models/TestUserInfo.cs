namespace FitTech.Api.Tests.Models;

public class TestUserInfo
{
    public static string SharedKey = "TestUserInfo";
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}
