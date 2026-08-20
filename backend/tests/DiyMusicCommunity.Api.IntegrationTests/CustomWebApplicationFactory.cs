using DiyMusicCommunity.Infrastructure.Persistence;
using DiyMusicCommunity.Application.Abstractions;
using DiyMusicCommunity.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
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
            services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
            services.RemoveAll<IImageUrlResolver>();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(_dbName)
                .Options;

            services.AddSingleton<DbContextOptions<AppDbContext>>(options);
            services.AddSingleton<DbContextOptions>(options);
            services.AddScoped<AppDbContext>();
            services.AddScoped<IImageUrlResolver, TestImageUrlResolver>();
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
        SeedGenres(db);

        if (seed is not null)
        {
            seed(db);
            db.SaveChanges();
        }

        return client;
    }

    private static void SeedGenres(AppDbContext db)
    {
        if (db.Genres.Any())
        {
            return;
        }

        db.Genres.AddRange(
        [
            new Genre(new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), "Grindcore"),
            new Genre(new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901"), "Crust"),
            new Genre(new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012"), "D-Beat"),
            new Genre(new Guid("d4e5f6a7-b8c9-0123-def0-234567890123"), "Powerviolence"),
            new Genre(new Guid("e5f6a7b8-c9d0-1234-ef01-345678901234"), "Punk"),
            new Genre(new Guid("f6a7b8c9-d0e1-2345-f012-456789012345"), "Noise"),
            new Genre(new Guid("a7b8c9d0-e1f2-3456-0123-567890123456"), "Goregrind"),
            new Genre(new Guid("b8c9d0e1-f2a3-4567-1234-678901234567"), "Gorenoise"),
            new Genre(new Guid("c9d0e1f2-a3b4-5678-2345-789012345678"), "Death Metal"),
            new Genre(new Guid("d0e1f2a3-b4c5-6789-3456-890123456789"), "Death-Grind")
        ]);
        db.SaveChanges();
    }

    private sealed class TestImageUrlResolver : IImageUrlResolver
    {
        public Task<string?> ResolveAsync(string? blobPath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(blobPath))
            {
                return Task.FromResult<string?>(null);
            }

            return Task.FromResult<string?>("https://example.test/" + blobPath);
        }
    }
}
