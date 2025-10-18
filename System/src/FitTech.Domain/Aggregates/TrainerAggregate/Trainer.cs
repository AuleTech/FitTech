﻿using AuleTech.Core.Patterns.Result;
using FitTech.Domain.Aggregates.ClientAggregate;
using FitTech.Domain.Enums;
using FitTech.Domain.Seedwork;

namespace FitTech.Domain.Aggregates.TrainerAggregate;

public class Trainer : Entity, IAggregateRoot
{
    internal Trainer()
    {
    }

    public string Name { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;

    public virtual List<Client> Clients { get; set; } = [];
    public virtual List<Invitation> Invitations { get; set; } = [];

    public static Result<Trainer> Create(string name, string lastName, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<Trainer>.Failure($"{nameof(Name)} cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            return Result<Trainer>.Failure($"{nameof(LastName)} cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return Result<Trainer>.Failure($"{nameof(Email)} cannot be empty");
        }

        return new Trainer
        {
            Name = name, LastName = lastName, Email = email.ToLowerInvariant(), Id = Guid.CreateVersion7()
        };
    }

    public Result<Invitation> InviteClient(string clientEmail)
    {
        if (HasInProgressInvitations())
        {
            return Result<Invitation>.Failure("There is already existing invitations");
        }
        
        var invitationResult = Invitation.Create(Id, clientEmail);

        if (!invitationResult.Succeeded)
        {
            return invitationResult;
        }
        
        Invitations.Add(invitationResult.Value!);
        
        return Result<Invitation>.Success(invitationResult.Value!);

        bool HasInProgressInvitations()
        {
            return Invitations.Any(x => x.Email == clientEmail && x.Status is InvitationStatus.Pending or InvitationStatus.InProgress);
        }
    }
    
    public Result SetInvitationInProgress(string email, int code)
    {
        var invitationResult = GetInvitationByEmailAndCode(email, code);

        if (!invitationResult.Succeeded)
        {
            return invitationResult;
        }
        
        invitationResult.Value!.SetInProgress();
        
        return Result.Success;
    }

    public Result<Invitation> GetInvitationByEmailAndCode(string email, int code)
    {
        var invitation = Invitations.FirstOrDefault(x => x.Email == email && x.Code == code);

        if (invitation is null)
        {
            return Result<Invitation>.Failure("Invitation not found");
        }

        return invitation;
    }

    public Result CanRegisterClient(Guid invitationId, string email)
    {
        var invitation = Invitations.Single(x => x.Id == invitationId);

        if (invitation.Email != email)
        {
            return Result.Failure("Email doesn't match. You need to use the same email where you got the invitation");
        }
        
        return Result.Success;
    }

    public Result CompleteClientRegistration(Guid clientId)
    {
        var client = Clients.SingleOrDefault(x => x.Id == clientId);

        if (client is null)
        {
            return Result.Failure("Client not found");
        }
        
        var invitation = Invitations.SingleOrDefault(x => x.Email == client.Email && x.Status == InvitationStatus.InProgress);

        if (invitation is null)
        {
            return Result.Failure("Invitation not found");
        }
        
        return invitation.SetCompleted(); 
    }
}
