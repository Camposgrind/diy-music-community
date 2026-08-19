using DiyMusicCommunity.Application.Abstractions;
using DiyMusicCommunity.Domain.Abstractions;
using DiyMusicCommunity.Infrastructure.Auth;
using DiyMusicCommunity.Infrastructure.Persistence;
using DiyMusicCommunity.Infrastructure.Repositories;
using DiyMusicCommunity.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiyMusicCommunity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is not configured. " +
                "Set it via 'dotnet user-secrets' for local development or Azure Key Vault for deployed environments.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<IBandRepository, BandRepository>();
        services.AddScoped<IReleaseRepository, ReleaseRepository>();
        services.AddScoped<ICatalogDeletionRepository, CatalogDeletionRepository>();
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<IProposalRepository, ProposalRepository>();
        services.AddScoped<IClaimRepository, ClaimRepository>();

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.Configure<AzureStorageOptions>(options =>
        {
            options.ConnectionString = configuration["AzureStorage:ConnectionString"] ?? string.Empty;
            options.ContainerName = configuration["AzureStorage:ContainerName"] ?? string.Empty;
            options.SasLifetimeDays = ParseInt(configuration["AzureStorage:SasLifetimeDays"]);
        });
        services.Configure<FileUploadOptions>(options =>
        {
            options.MaxImageSizeMb = ParseInt(configuration["FileUpload:MaxImageSizeMb"]);
            options.TemporaryFileLifetimeMinutes = ParseInt(configuration["FileUpload:TemporaryFileLifetimeMinutes"]);
            options.TemporaryDirectory = configuration["FileUpload:TemporaryDirectory"];
        });
        services.AddScoped<IBlobStorageService, AzureBlobStorageService>();
        services.AddScoped<IImageUrlResolver, BandImageUrlResolver>();
        services.AddScoped<IImageFileValidator, ImageFileValidator>();
        services.AddScoped<IImageUploadSettings, ImageUploadSettings>();
        services.AddScoped<ITemporaryImageStorage, LocalTemporaryImageStorage>();

        return services;
    }

    private static int ParseInt(string? value)
    {
        return int.TryParse(value, out var result) ? result : 0;
    }
}


