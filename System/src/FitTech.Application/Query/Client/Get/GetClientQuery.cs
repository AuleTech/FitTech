using AuleTech.Core.Patterns.CQRS;

namespace FitTech.Application.Query.Client.Get;

public record GetClientDataQuery(Guid Id) : IQuery;

public record ClientDataDto(
    string Name,
    string LastName,
    string Email,
    DateTimeOffset Birthdate,
    int TrainingHours,
    string TrainingModel,
    DateTimeOffset EventDate,
    string Center,
    string SubscriptionType);
