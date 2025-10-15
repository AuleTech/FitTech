namespace FitTech.Domain.Services;

internal class InvitationCodeGenerator
{
    public static InvitationCodeGenerator Instance = new();
    private readonly Random _random = Random.Shared;
    public int Generate()
    {
        return _random.Next(999999);
    }
}
