using Bogus;
using FitTech.Domain.Aggregates.ClientAggregate;
using FitTech.Domain.Aggregates.TrainerAggregate;
using FitTech.Domain.Enums;
using FitTech.TestingSupport;
using FitTech.TestingSupport.Faker;

namespace FitTech.UnitTests.Data.Generators;

internal sealed class TrainerFakeGenerator : FakerWithPrivateConstructor<Trainer>
{
    private readonly InvitationFakeGenerator _invitationFakeGenerator = new ();
    public TrainerFakeGenerator()
    {
        RuleFor(x => x.Email, x => x.Person.Email);
        RuleFor(x => x.Id, _ => Guid.CreateVersion7());
        RuleFor(x => x.Name, x => x.Person.FirstName);
        RuleFor(x => x.LastName, x => x.Person.LastName);
    }

    public TrainerFakeGenerator WithInvitations()
    {
        RuleFor(x => x.Invitations, _ => _invitationFakeGenerator.Generate(10));
        
        return this;
    }
    
    public TrainerFakeGenerator WithInvitations(IEnumerable<Invitation> invitations)
    {
        RuleFor(x => x.Invitations, _ => invitations.ToList());
        
        return this;
    }

    public TrainerFakeGenerator WithClients(IEnumerable<Client> clients)
    {
        RuleFor(x => x.Clients, _ => clients.ToList());

        return this;
    }
}
