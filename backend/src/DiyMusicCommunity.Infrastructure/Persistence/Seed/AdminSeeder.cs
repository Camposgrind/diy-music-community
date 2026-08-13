using DiyMusicCommunity.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiyMusicCommunity.Infrastructure.Persistence.Seed;

/// <summary>
/// Promotes the configured admin email to the Admin role at startup.
/// The email is read from Seed:AdminEmail (set via user-secrets or Key Vault).
/// Idempotent — safe to run on every startup.
/// </summary>
public static class AdminSeeder
{
    public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var adminEmail = configuration["Seed:AdminEmail"];

        if (string.IsNullOrWhiteSpace(adminEmail))
        {
            return;
        }

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(nameof(AdminSeeder));

        var user = await userManager.FindByEmailAsync(adminEmail);
        if (user is null)
        {
            logger.LogWarning(
                "Admin seed skipped: no user found with email '{Email}'. Register first, then restart.",
                adminEmail);
            return;
        }

        var isAlreadyAdmin = await userManager.IsInRoleAsync(user, "Admin");
        if (isAlreadyAdmin)
        {
            return;
        }

        var result = await userManager.AddToRoleAsync(user, "Admin");
        if (result.Succeeded)
        {
            logger.LogInformation("User '{Email}' promoted to Admin.", adminEmail);
        }
        else
        {
            logger.LogError(
                "Failed to promote '{Email}' to Admin: {Errors}",
                adminEmail,
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
