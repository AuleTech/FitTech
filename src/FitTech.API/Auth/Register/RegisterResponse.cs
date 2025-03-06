namespace FitTech.API.Auth.Register;

public record RegisterResponse(bool Succeeded, IEnumerable<RegisterErrors> Errors);
public record RegisterErrors(string Code, string Description);
