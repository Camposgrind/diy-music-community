using DiyMusicCommunity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DiyMusicCommunity.Api.IntegrationTests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    // Unique DB name per factory instance — each test class gets its own
    // isolated in-memory store without interfering with SQL Server migrations.
    private readonly string _dbName = $"TestDb_{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the AppDbContext and its options registered by AddInfrastructure.
            // We must NOT call AddDbContext again because that would add InMemory provider
            // services on top of the already-registered SQL Server ones, causing EF to throw.
            services.RemoveAll<AppDbContext>();
            services.RemoveAll<DbContextOptions<AppDbContext>>();

            // Build options in isolation — DbContextOptionsBuilder creates its own internal
            // EF service provider scoped to these options only, so the SQL Server services
            // that are already in the DI container are never seen by this context.
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(_dbName)
                .Options;

            // Register the pre-built options so AppDbContext can resolve them via DI.
            services.AddSingleton<DbContextOptions<AppDbContext>>(options);
            services.AddSingleton<DbContextOptions>(options);

            // Re-register AppDbContext as scoped (same lifetime as the original).
            services.AddScoped<AppDbContext>();
        });

        builder.UseEnvironment("Development");
    }

    /// <summary>
    /// Creates an <see cref="HttpClient"/> and optionally seeds the database before the test.
    /// </summary>
    public HttpClient CreateClientWithDb(Action<AppDbContext>? seed = null)
    {
        var client = CreateClient();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // EnsureCreated with in-memory: instant, no SQL, no migration involvement.
        db.Database.EnsureCreated();

        if (seed is not null)
        {
            seed(db);
            db.SaveChanges();
        }

        return client;
    }
}
