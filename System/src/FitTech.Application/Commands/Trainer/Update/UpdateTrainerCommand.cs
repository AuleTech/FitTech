﻿using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Domain.Repositories;

namespace FitTech.Application.Commands.Trainer.Update;

public class UpdateTrainerCommand : ICommand
{
    public Guid Id { get; set; }
    public string Name { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;

}

public static class UpdateTrainerCommandExtensions
{
    public static Domain.Entities.Trainer ToEntity(this UpdateTrainerCommand command) => new Domain.Entities.Trainer
    {
        Name = command.Name,
        Email = command.Email,
        Password = command.Password,
    };
} 
