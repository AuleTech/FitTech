using FitTech.Identity.API.Models;

namespace FitTech.Identity.API.Data;

/// <remarks>
/// Add migrations using the following command inside the 'FitTech.Identity.API' project directory:
///
/// dotnet ef migrations add [migration-name]
/// </remarks>
public class FitTechIdentityDbContext : IdentityDbContext<ApplicationUser>
{
    public FitTechIdentityDbContext(DbContextOptions<FitTechIdentityDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
