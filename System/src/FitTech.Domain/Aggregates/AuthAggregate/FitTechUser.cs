using Microsoft.AspNetCore.Identity;

namespace FitTech.Domain.Aggregates.AuthAggregate;

public class FitTechUser : IdentityUser<Guid>
{
}
