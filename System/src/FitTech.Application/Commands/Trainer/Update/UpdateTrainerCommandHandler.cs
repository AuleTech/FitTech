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
        var trainer  = await _userManager.FindByIdAsync(command.Id.ToString()).WaitAsync(cancellationToken);

        if (trainer is null)
        {
            return Result<TrainerDataDto>.Failure("Trainer not found");
        }
        var update = await _trainerRepository.UpdateTrainerAsync(command.Id, command.Name, command.Email, command.Password, cancellationToken);
        
        update?.UpdateData(command.ToEntity(), cancellationToken);
        
        return Result.Success;
    } 
}
