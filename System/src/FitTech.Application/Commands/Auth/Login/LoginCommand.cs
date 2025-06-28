using AuleTech.Core.Patterns.CQRS;

namespace FitTech.Application.Commands.Auth.Login;

public record LoginCommand(string Email, string Password): ICommand;

public record LoginResultDto(string AccessToken, string RefreshToken);
