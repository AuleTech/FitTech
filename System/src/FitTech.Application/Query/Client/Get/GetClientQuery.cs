using AuleTech.Core.Patterns.CQRS;

namespace FitTech.Application.Query.Client.Get;

public record GetClientDataQuery(Guid Id) : IQuery;

public record ClientDataDto(
    string Name,
    string LastName,
    string Email,
    DateTime Birthdate,
    int TrainingHours,
    string TrainingModel,
    DateTime EventDate,
    string Center,
    string SubscriptionType);
