using AuleTech.Core.Patterns.CQRS;

namespace FitTech.Application.Commands.Auth.RequestPassword;

public record ResetPasswordCommand(string Email, string Password, string Token) : ICommand;
