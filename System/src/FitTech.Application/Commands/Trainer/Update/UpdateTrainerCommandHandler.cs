using AuleTech.Core.Patterns.CQRS;
using AuleTech.Core.Patterns.Result;
using FitTech.Application.Query.Trainer.GetTrainerData;
using FitTech.Domain.Entities;
using FitTech.Domain.Repositories;
using Microsoft.AspNetCore.Identity;

namespace FitTech.Application.Commands.Trainer.Update;

public class UpdateTrainerCommandHandler : IAuleTechCommandHandler<UpdateTrainerCommand, Result>
{
    private readonly ITrainerRepository _trainerRepository;
    private readonly UserManager<FitTechUser> _userManager;


    public UpdateTrainerCommandHandler(ITrainerRepository trainerRepository, UserManager<FitTechUser> userManager)
    {
        _trainerRepository = trainerRepository;
        _userManager = userManager;

    }

    public async Task<Result> HandleAsync(UpdateTrainerCommand command, CancellationToken cancellationToken)
    {
        
        var trainer = await _trainerRepository.GetByIdAsync(command.Id, cancellationToken);

        if (trainer is null)
        {
            return Result.Failure("Trainer not found.");
        }

        trainer.UpdateData(command.Name, command.Email, command.Password);

        var trainerupdate = await _trainerRepository.UpdateTrainerAsync(trainerId:command.Id, name:command.Name, email:command.Email, password:command.Password, cancellationToken);

        return Result.Success;
    } 
}
