using DiyMusicCommunity.Application.Auth.Login;
using DiyMusicCommunity.Application.Auth.Register;
using DiyMusicCommunity.Application.Bands.GetBandDetail;
using DiyMusicCommunity.Application.Bands.GetBands;
using DiyMusicCommunity.Application.Bands.CatalogManagement;
using DiyMusicCommunity.Application.Bands.CatalogDeletion;
using DiyMusicCommunity.Application.Bands.Images;
using DiyMusicCommunity.Application.Genres.GetGenres;
using DiyMusicCommunity.Application.Releases.GetReleaseDetail;
using DiyMusicCommunity.Application.Releases.Images;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DiyMusicCommunity.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<GetBandsQueryValidator>();
        services.AddScoped<GetBandsUseCase>();
        services.AddScoped<GetBandDetailUseCase>();
        services.AddScoped<CatalogManagementUseCase>();
        services.AddScoped<CatalogDeletionUseCase>();
        services.AddScoped<BandImagesUseCase>();
        services.AddScoped<GetReleaseDetailUseCase>();
        services.AddScoped<ReleaseImagesUseCase>();
        services.AddScoped<GetGenresUseCase>();
        services.AddScoped<RegisterUseCase>();
        services.AddScoped<LoginUseCase>();

        return services;
    }
}

