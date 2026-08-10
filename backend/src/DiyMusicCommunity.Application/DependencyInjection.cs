using DiyMusicCommunity.Application.Bands.GetBandDetail;
using DiyMusicCommunity.Application.Bands.GetBands;
using DiyMusicCommunity.Application.Releases.GetReleaseDetail;
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
        services.AddScoped<GetReleaseDetailUseCase>();

        return services;
    }
}
