using AuleTech.Core.Patterns.CQRS;

namespace FitTech.Application.Query.Trainer.GetTrainerData;

public record GetTrainerDataQuery(Guid Id) : IQuery;

public record TrainerDataDto(string TrainerName,  string Email,  string Password);
