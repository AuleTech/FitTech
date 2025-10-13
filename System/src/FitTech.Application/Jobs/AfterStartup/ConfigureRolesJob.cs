using AuleTech.Core.System.Host;
using FitTech.Domain.Aggregates.AuthAggregate;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FitTech.Application.Jobs.AfterStartup;

internal sealed class ConfigureRolesJob : IAfterStartupJob
{
    private readonly ILogger<ConfigureRolesJob> _logger;
    private readonly RoleManager<FitTechRole> _roleManager;

    public ConfigureRolesJob(RoleManager<FitTechRole> roleManager, ILogger<ConfigureRolesJob> logger)
    {
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Start Running {nameof(ConfigureRolesJob)}");

        await AddRoleIfDoesNotExistsAsync(FitTechRole.Trainer);
        await AddRoleIfDoesNotExistsAsync(FitTechRole.Client);

        _logger.LogInformation($"{nameof(ConfigureRolesJob)} execution succeeded");

        async Task AddRoleIfDoesNotExistsAsync(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new FitTechRole { Name = roleName, Id = Guid.CreateVersion7() });
            }
        }
    }
}
