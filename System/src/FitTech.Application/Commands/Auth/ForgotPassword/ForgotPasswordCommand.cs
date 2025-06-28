using AuleTech.Core.Patterns.CQRS;

namespace FitTech.Application.Commands.Auth.ForgotPassword;

public record ForgotPasswordCommand(string Email, string CallbackUrl) : ICommand;
