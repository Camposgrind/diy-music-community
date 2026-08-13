using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiyMusicCommunity.Infrastructure.Persistence.Seed;

/// <summary>
/// Seeds the default roles into the Identity store at startup.
/// Idempotent — roles are only created if they do not already exist.
/// </summary>
public static class RoleSeeder
{
    private static readonly string[] Roles = ["Admin", "Moderator", "User"];

    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(nameof(RoleSeeder));

        foreach (var roleName in Roles)
        {
            var exists = await roleManager.RoleExistsAsync(roleName);
            if (!exists)
            {
                var result = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName)
                {
                    Id = Guid.NewGuid()
                });

                if (result.Succeeded)
                {
                    logger.LogInformation("Role '{Role}' created.", roleName);
                }
                else
                {
                    logger.LogError("Failed to create role '{Role}': {Errors}",
                        roleName,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
