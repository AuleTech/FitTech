﻿using FitTech.Domain.Aggregates.TrainerAggregate;
using FitTech.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FitTech.Persistence.Repositories;

internal class TrainerRepository : ITrainerRepository
{
    private readonly FitTechDbContext _context;

    public TrainerRepository(FitTechDbContext context)
    {
        _context = context;
    }

    public async Task<Trainer> AddAsync(Trainer trainer, CancellationToken cancellationToken)
    {
        await _context.Trainers.AddAsync(trainer, cancellationToken);

        return trainer;
    }

    public async Task<Invitation> AddInvitationAsync(Invitation invitation, CancellationToken cancellationToken)
    {
        await _context.Invitations.AddAsync(invitation, cancellationToken);

        return invitation;
    }

    public async Task<Trainer?> GetAsync(Guid trainerId, CancellationToken cancellationToken)
    {
        return await _context.Trainers
            .Include(x => x.Invitations)
            .Include(x => x.Clients)
            .SingleOrDefaultAsync(x => x.Id == trainerId, cancellationToken);
    }
}
