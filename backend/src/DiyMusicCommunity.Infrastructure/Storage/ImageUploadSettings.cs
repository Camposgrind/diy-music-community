using DiyMusicCommunity.Application.Abstractions;
using Microsoft.Extensions.Options;

namespace DiyMusicCommunity.Infrastructure.Storage;

public sealed class ImageUploadSettings : IImageUploadSettings
{
    private readonly FileUploadOptions _options;

    public ImageUploadSettings(IOptions<FileUploadOptions> options)
    {
        _options = options.Value;
    }

    public TimeSpan TemporaryFileLifetime
    {
        get
        {
            return TimeSpan.FromMinutes(_options.TemporaryFileLifetimeMinutes);
        }
    }
}
