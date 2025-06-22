using Microsoft.AspNetCore.Identity;

namespace FitTech.Domain.Entities;

public class FitTechRole : IdentityRole<Guid>
{
    public static string Trainer = "Trainer";
    public static string Client = "Client";
}
