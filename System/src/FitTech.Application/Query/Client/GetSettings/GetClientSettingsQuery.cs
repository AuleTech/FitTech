using AuleTech.Core.Patterns.CQRS;

namespace FitTech.Application.Query.Client.GetSettings;

public record GetClientSettingsQuery(Guid Id) : IQuery;
public record ClientSettingsDto(string Name, string TrainerName, string Email);
