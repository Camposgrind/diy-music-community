using DiyMusicCommunity.Domain.Abstractions;
using DiyMusicCommunity.Infrastructure.Persistence;
using DiyMusicCommunity.Infrastructure.Repositories;
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
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProposalRepository, ProposalRepository>();
        services.AddScoped<IClaimRepository, ClaimRepository>();

        return services;
    }
}
